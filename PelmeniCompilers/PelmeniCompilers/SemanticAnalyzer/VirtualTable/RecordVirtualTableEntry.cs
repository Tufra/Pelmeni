using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record RecordVirtualTableEntry
{
    public string Name { get; set; } = null!;
    public List<VariableVirtualTableEntry> Members { get; set; } = new();
    public int FieldOffset { get; set; } = 0;
    public Node Node { get; set; } = null!;
}