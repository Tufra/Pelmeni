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

    public static Dictionary<string, RoutineVirtualTableEntry> RoutineVirtualTable { get; set; }
    public static Dictionary<string, RoutineVirtualTableEntry> LibRoutineVirtualTable { get; set; }
    public static Dictionary<string, RecordVirtualTableEntry> RecordVirtualTable { get; set; }

    static BaseNodeRuleChecker()
    {
        Scope = new Scope();
        Chain = new Stack<Node>();
        RoutineVirtualTable = new Dictionary<string, RoutineVirtualTableEntry>();
        RecordVirtualTable = new Dictionary<string, RecordVirtualTableEntry>();
        LibRoutineVirtualTable = new Dictionary<string, RoutineVirtualTableEntry>();
    }

    public abstract void Check(Node node);
    public virtual ComputedExpression BuildComputedExpression(Node node)
    {
        throw new InvalidOperationException("Not available for this node");
    }

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

    public static RoutineVirtualTableEntry GetRoutineOrThrowIfNotDeclared(Node node)
    {
        var identifier = node.Children[0].Token!.Value;
        var location = node.Children[0].Token!.Location;

        if (RoutineVirtualTable.TryGetValue(identifier, out var declared))
        {
            return declared;
        }

        if (LibRoutineVirtualTable.TryGetValue(identifier, out var declaredInLib))
        {
            return declaredInLib;
        }
        
        throw new InvalidOperationException($"Routine {identifier} at {location} was not declared");
    }

    protected static RecordVirtualTableEntry GetRecordOrThrowIfNotDeclared(string identifier, LexLocation location)
    {
        if (!RecordVirtualTable.ContainsKey(identifier))
            throw new InvalidOperationException($"Record {identifier} at {location} was not declared");
        
        return RecordVirtualTable[identifier];
    }

    protected static VariableVirtualTableEntry GetVariableOrThrowIfNotDeclared(Node token)
    {
        var identifier = token.Token!.Value;
        var location = token.Token!.Location;

        if (!Scope.Contains(identifier))
            throw new InvalidOperationException($"Variable {identifier} at {location} was not declared");

        return Scope.Get(identifier)!;
    }
}