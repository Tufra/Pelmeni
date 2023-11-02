using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Module : ScopeUnit
{
    public Module(string name, Token token) : base(name, token)
    {
    }
}