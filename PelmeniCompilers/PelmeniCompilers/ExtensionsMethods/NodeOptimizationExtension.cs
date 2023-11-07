using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeOptimizationExtension
{
    internal static Dictionary<Node, bool> VariableUsage { get; set; } = new();
    internal static Dictionary<Node, bool> RecordUsage { get; set; } = new();

    public static void Optimize(this Node node)
    {
        node.DeleteUnusedVariables();
        node.DeleteUnusedRecords();
    }

    private static void DeleteUnusedVariables(this Node node)
    {
        if (VariableUsage.Keys.Any(n => n.Type != NodeType.VariableDeclaration))
        {
            throw new InvalidOperationException($"There is no variable in the {nameof(VariableUsage)} dictionary");
        }

        var unusedVariables = VariableUsage.Where(entry => !entry.Value).Select(entry => entry.Key).ToList();
        unusedVariables.ForEach(node.RemoveInChildren);
    }
    
    private static void DeleteUnusedRecords(this Node node)
    {
        if (RecordUsage.Keys.Any(n => n.Type != NodeType.RecordType))
        {
            throw new InvalidOperationException($"There is no record in the {nameof(RecordUsage)} dictionary");
        }

        var unusedVariables = RecordUsage.Where(entry => !entry.Value).Select(entry => entry.Key).ToList();
        unusedVariables.ForEach(unusedVariable => node.Children.Remove(unusedVariable));
    }
}