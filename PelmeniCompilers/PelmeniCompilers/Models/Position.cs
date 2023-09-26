namespace PelmeniCompilers.Models;

public record Position
{
    public long LineNumber { get; protected set; }
    public int PositionBegin { get; protected set; }

    public Position(long lineNumber, int positionBegin)
    {
        LineNumber = lineNumber;
        PositionBegin = positionBegin;
    }

    /*public override string ToString()
    {
        return $"({LineNumber}:{PositionBegin})";
    }*/
}