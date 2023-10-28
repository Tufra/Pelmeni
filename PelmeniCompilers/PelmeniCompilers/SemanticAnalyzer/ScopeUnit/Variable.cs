using PelmeniCompilers.Enum;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit
{
    public record Variable : Unit
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
    }
}