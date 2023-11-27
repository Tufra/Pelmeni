using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record RoutineVirtualTableEntry : IComparable<RoutineVirtualTableEntry>
{
    public static int IndexCounter = 0;

    public int Index { get; set; }
    public string Name { get; set; } = null!;
    public List<VariableVirtualTableEntry> Parameters { get; set; } = new();
    public int LocalVariablesCounter => _bodyNode!.CountChildrenOfType(NodeType.VariableDeclaration);
    public int ForLoopsCounter => _bodyNode!.CountChildrenOfType(NodeType.ForLoop);
    public int ForeachLoopsCounter => _bodyNode!.CountChildrenOfType(NodeType.ForeachLoop);
    public string ReturnType { get; set; } = null!;
    
    private Node? _bodyNode;

    public RoutineVirtualTableEntry()
    {
        Index = IndexCounter++;
    }

    public RoutineVirtualTableEntry(Node bodyNodeChild) : this()
    {
        bodyNodeChild.ThrowIfNodeNotTypeOf(NodeType.Body);
        _bodyNode = bodyNodeChild;
    }

    public int CompareTo(RoutineVirtualTableEntry? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Index.CompareTo(other.Index);
    }
}