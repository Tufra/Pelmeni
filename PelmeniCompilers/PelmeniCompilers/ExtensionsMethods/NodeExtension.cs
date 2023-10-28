using PelmeniCompilers.Models;

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
}