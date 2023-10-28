using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Rules;

public abstract class BaseSemanticRule
{
    public abstract NodeType CheckingNodeType { get; }

    public abstract void Check(Stack<HashSet<>> frames);

    public bool CanCheck(NodeType type) => type == CheckingNodeType;
}

public record Variable
{
    
}

public record Routine
{
    
}

public record Record
{
    
}