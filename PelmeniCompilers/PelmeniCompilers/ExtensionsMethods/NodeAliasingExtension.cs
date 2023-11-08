using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeAliasingExtension
{
    private static readonly Dictionary<string, Node> Aliasing;
    private static List<Node> _aliasingNodes = new();

    static NodeAliasingExtension()
    {
        Aliasing = new Dictionary<string, Node>();
    }

    public static void RemoveAliasing(this Node node)
    {
        if (!node.TryDeclareAlias())
        {
            node.TryReplaceAlias();
        }

        foreach (var child in node.Children)
        {
            child.RemoveAliasing();
        }
        
        if(node.Type == NodeType.Program)
            _aliasingNodes.ForEach(alias => node.Children.Remove(alias));
    }

    private static bool TryDeclareAlias(this Node node)
    {
        if (node.Type != NodeType.TypeDeclaration) return false;

        var alias = node.Children[0].Token!.Value;
        var type = node.Children[1];

        if (type.Type is not (NodeType.ArrayType or NodeType.Token)) return false;
        
        Aliasing[alias] = type;
        _aliasingNodes.Add(node);
        return true;

    }

    private static void TryReplaceAlias(this Node node)
    {
        if (node.Type != NodeType.TypeTail) return;
        if (node.Children.Count == 0) return;

        var type = node.Children[0].Type is NodeType.ArrayType
            ? node.Children[0].Children[0].Token!.Value
            : node.Children[0].Token!.Value;

        if (Aliasing.TryGetValue(type, out var value))
            node.Children = new List<Node>() { value };
    }
}