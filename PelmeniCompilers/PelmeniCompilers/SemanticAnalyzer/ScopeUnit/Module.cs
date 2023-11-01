namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Module : ScopeUnit
{
    public Module(string name) : base(name)
    {
    }
}