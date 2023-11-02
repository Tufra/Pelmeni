using PelmeniCompilers.Values;

namespace PelmeniCompilers.Models;

public record ComputedExpression : Node
{
    public ComputedExpression(NodeType type, Token? token, string valueType, string? value) : base(type, token)
    {
        ValueType = valueType;
        if (value is not null)
        {
            Value = value;
        }
    }

    public string ValueType { get; set; }
    public string? Value { get; set; }
}