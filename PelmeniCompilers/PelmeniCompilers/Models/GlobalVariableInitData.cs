namespace PelmeniCompilers.Models;

public record GlobalVariableInitData
{
    public Node? Type { get; set; }
    public Node? InitTail { get; set; }
};