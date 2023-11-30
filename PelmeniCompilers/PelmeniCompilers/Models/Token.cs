using System.Text.Json.Serialization;
using PelmeniCompilers.ShiftReduceParser;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Models;

public record Token
{
    public LexLocation Location { get; set; } = null!;
    public string Value { get; set; } = null!;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TokenType TokenType { get; set; }
}