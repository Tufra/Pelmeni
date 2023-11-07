using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record VariableVirtualTableEntry
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public Node Node { get; set; } = null!;
}