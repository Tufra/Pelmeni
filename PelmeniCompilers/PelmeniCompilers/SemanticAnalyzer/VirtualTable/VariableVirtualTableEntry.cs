namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record VariableVirtualTableEntry
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
}