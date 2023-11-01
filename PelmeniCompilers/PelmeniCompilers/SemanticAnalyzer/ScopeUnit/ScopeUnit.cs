namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record ScopeUnit
{
    public string Name { get; set; }

    public ScopeUnit(string name)
    {
        Name = name;
    }
}