using PelmeniCompilers.Enum;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit;

public record Array : ScopeUnit
{
    public List<int> Size { get; set; }
    public TokenType Type { get; set; }

    public Array(string name) : base(name)
    {
    }
}