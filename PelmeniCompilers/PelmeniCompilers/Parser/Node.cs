using System.Collections;
using System.Diagnostics;
using System.Xml;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Parser;

public record Node
{
    public NodeType Type { get; private set; }
    public Token? Token { get; private set; }
    public List<Node>? Children { get; private set; }

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

    public bool IsTerminal()
    {
        return Children is null;
    }
}