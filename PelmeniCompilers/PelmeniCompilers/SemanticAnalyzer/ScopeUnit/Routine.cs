using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Routine : ScopeUnit
{
    public List<Variable> Parameters { get; set; }
    public Variable? ReturnType { get; set; }


    public Routine(string name, Token token) : base(name, token)
    {
    }
}