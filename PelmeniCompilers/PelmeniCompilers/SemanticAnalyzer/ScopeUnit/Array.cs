using PelmeniCompilers.Enum;
using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Array : ScopeUnit
{
    public List<int> Size { get; set; }
    public TokenType Type { get; set; }


    public Array(string name, Token token) : base(name, token)
    {
    }
}