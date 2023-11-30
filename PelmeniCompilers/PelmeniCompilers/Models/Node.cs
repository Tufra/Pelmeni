using System.Text.Json.Serialization;
using ConsoleTree;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Models;

public record Node
{
    public Node()
    {
    }

    public Node(NodeType type, Token? token)
    {
        Type = type;
        Token = token;
    }

    public Node(NodeType type, List<Node> children)
    {
        Type = type;
        Children = children;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NodeType Type { get;  set; }
    public Token? Token { get;  set; }
    public List<Node> Children { get; set; } = new();

    public bool IsTerminal()
    {
        return Children.Count == 0;
    }
}