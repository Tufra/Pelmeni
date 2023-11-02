namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record RecordVirtualTableEntry
{
    public string Name { get; set; }
    public List<VariableVirtualTableEntry> Members { get; set; } = new();
}