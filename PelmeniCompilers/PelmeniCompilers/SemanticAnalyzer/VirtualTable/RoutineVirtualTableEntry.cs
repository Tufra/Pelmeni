namespace PelmeniCompilers.SemanticAnalyzer.VirtualTable;

public record RoutineVirtualTableEntry : IComparable<RoutineVirtualTableEntry>
{
    public static int IndexCounter = 0;
    
    public int Index { get; set; }
    public string Name { get; set; } = null!;
    public List<VariableVirtualTableEntry> Parameters { get; set; } = new();
    public int LocalVariablesCounter { get; set; }
    public string ReturnType { get; set; } = null!;

    public RoutineVirtualTableEntry()
    {
        Index = IndexCounter++;
    }

    public int CompareTo(RoutineVirtualTableEntry? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Index.CompareTo(other.Index);
    }
}