using PelmeniCompilers.Enum;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit
{
    public record Variable : ScopeUnit
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Variable(string name) : base(name)
        {
        }
    }
}