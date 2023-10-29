namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Unit
{
    public string Name { get; set; }

    public Unit(string name)
    {
        Name = name;
    }
}