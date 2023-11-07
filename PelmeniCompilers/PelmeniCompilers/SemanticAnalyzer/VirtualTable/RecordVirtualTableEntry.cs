using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record RecordVirtualTableEntry
{
    public string Name { get; set; } = null!;
    public List<VariableVirtualTableEntry> Members { get; set; } = new();
    public Node Node { get; set; } = null!;
}