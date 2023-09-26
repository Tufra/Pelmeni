using PelmeniCompilers.Values;

namespace PelmeniCompilers.Models;

public record Token
{
    public Position Position { get; set; } = null!;
    public string Value { get; set; } = null!;
    public TokenType TokenType { get; set; }
}