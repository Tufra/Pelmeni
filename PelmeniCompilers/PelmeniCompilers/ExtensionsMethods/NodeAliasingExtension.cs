using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeAliasingExtension
{
    private static readonly Dictionary<string, Node> Aliasing;

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
    }

    private static bool TryDeclareAlias(this Node node)
    {
        if (node.Type != NodeType.TypeDeclaration) return false;

        var alias = node.Children[0].Token!.Value;
        var type = node.Children[1];

        if (type.Type is not (NodeType.RecordType or NodeType.ArrayType or NodeType.Token)) return false;
        
        Aliasing[alias] = type;
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