namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Record : Unit 
{
    /// <summary>
    /// Properties
    /// </summary>
    public List<Variable> Members { get; set; }
}