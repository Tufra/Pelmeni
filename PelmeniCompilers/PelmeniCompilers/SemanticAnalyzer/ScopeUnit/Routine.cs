using PelmeniCompilers.SemanticAnalyzer.Rules;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Routine : Unit
{
    public List<Variable> Parameters { get; set; }
    public Variable? ReturnType { get; set; }
}