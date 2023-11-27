using System.Collections;
using System.Reflection;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeExtension
{
    public static void ThrowIfNodeNotTypeOf(this Node node, NodeType type)
    {
        if (node.Type != type)
            throw new InvalidOperationException($"{node} is expected to be {type}");
    }
    
    public static int CountChildrenOfType(this Node node, NodeType type)
    {
        return node.Type == type ? 1 : node.Children.Sum(child => child.CountChildrenOfType(type));
    }

    public static void RemoveInChildren(this Node node, Node deletingNode)
    {
        var children = node.Children;
        if (children.Remove(deletingNode))
            return;
        
        children.ForEach(child => child.RemoveInChildren(deletingNode));
    }
}