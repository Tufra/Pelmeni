using PelmeniCompilers.ShiftReduceParser;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Models;

public record Token
{
    public LexLocation Location { get; set; } = null!;
    public string Value { get; set; } = null!;
    public TokenType TokenType { get; set; }
}