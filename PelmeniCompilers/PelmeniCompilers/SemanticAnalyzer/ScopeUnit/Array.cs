using PelmeniCompilers.Enum;
using PelmeniCompilers.SemanticAnalyzer.Rules;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Array : Unit
{
    public List<int> Size { get; set; }
    public TokenType Type { get; set; }
}