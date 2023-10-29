using ConsoleTree;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Models;

public record Node
{
    public Node(NodeType type, Token? token)
    {
        Type = type;
        Token = token;
    }

    public Node(NodeType type, List<Node>? children)
    {
        Type = type;
        Children = children;
    }

    public NodeType Type { get; private set; }
    public Token? Token { get; private set; }
    public List<Node>? Children { get; }

    public bool IsTerminal()
    {
        return Children is null;
    }
}