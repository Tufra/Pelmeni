namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Routine : Unit
{
    public List<Variable> Parameters { get; set; }
    public Variable? ReturnType { get; set; }

    public Routine(string name) : base(name)
    {
    }
}