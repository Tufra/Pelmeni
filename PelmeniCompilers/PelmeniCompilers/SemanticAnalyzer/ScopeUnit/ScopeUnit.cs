using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record ScopeUnit
{
    public string Name { get; set; }
    
    public Token Token { get; }

    public ScopeUnit(string name, Token token)
    {
        Name = name;
        Token = token;
    }
}