using PelmeniCompilers.Constants;

namespace PelmeniCompilers.Models;

public class Token
{
    public Position Position { get; set; } = null!;
    public string Value { get; set; } = null!;
    public TokenTypes TokenType { get; set; }
}

public class Position
{
    public long LineNumber { get; protected set; }
    public int PositionBegin { get; protected set; }

    public Position(long lineNumber, int positionBegin)
    {
        LineNumber = lineNumber;
        PositionBegin = positionBegin;
    }
}