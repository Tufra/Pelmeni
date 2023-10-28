using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeExtension
{
    public static void CheckSemantic(this Node node)
    {
        if (node.Children is not null)
        {
            foreach (var child in node.Children)
            {
                child.CheckSemantic();
            }
        }
            
        
    }

    public static void ThrowIfNodeNotTypeOf(this Node node, NodeType type)
    {
        if (node.Type != type)
            throw new InvalidOperationException($"{node} is expected to be {type}");
    }
}