namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record RoutineVirtualTableEntry
{
    public string Name { get; set; } = null!;
    public List<VariableVirtualTableEntry> Parameters { get; set; } = new();
    public string ReturnType { get; set; } = null!;
}