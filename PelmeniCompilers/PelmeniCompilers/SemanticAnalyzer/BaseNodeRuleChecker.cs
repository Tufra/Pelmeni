using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.ShiftReduceParser;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer;

public abstract class BaseNodeRuleChecker
{
    public abstract NodeType CheckingNodeType { get; }

    protected static Scope Scope { get; set; }
    protected static Stack<Node> Chain { get; set; }

    protected static Dictionary<string, RoutineVirtualTableEntry> RoutineVirtualTable { get; set; }
    protected static Dictionary<string, RecordVirtualTableEntry> RecordVirtualTable { get; set; }

    static BaseNodeRuleChecker()
    {
        Scope = new Scope();
        Chain = new Stack<Node>();
        RoutineVirtualTable = new Dictionary<string, RoutineVirtualTableEntry>();
        RecordVirtualTable = new Dictionary<string, RecordVirtualTableEntry>();
    }

    public abstract void Check(Node node);

    protected static void CheckChildren(Node node)
    {
        foreach (var child in node.Children)
        {
            child.CheckSemantic();
        }
    }

    protected static string GetIdentifierOrThrowIfOccupied(Node node)
    {
        var identifier = node.Children[0].Token!.Value;
        var location = node.Children[0].Token!.Location;

        if (RoutineVirtualTable.ContainsKey(identifier))
            throw new InvalidOperationException($"{identifier} at {location} is occupied by routine");
        
        if (RecordVirtualTable.ContainsKey(identifier))
            throw new InvalidOperationException($"{identifier} at {location} is occupied by record");
        
        if (Scope.Contains(identifier))
            throw new InvalidOperationException($"{identifier} at {location} is occupied by variable");

        return identifier;
    }
}