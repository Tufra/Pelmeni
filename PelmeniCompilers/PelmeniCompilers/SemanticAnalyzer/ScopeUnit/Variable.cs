using PelmeniCompilers.Enum;
using PelmeniCompilers.Models;

namespace PelmeniCompilers.SemanticAnalyzer.ScopeUnit
{
    public record Variable : ScopeUnit
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }


        public Variable(string name, Token token) : base(name, token)
        {
        }
    }
}