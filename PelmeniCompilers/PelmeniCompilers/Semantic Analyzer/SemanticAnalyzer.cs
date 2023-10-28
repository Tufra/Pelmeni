using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Semantic_Analyzer;

public class SemanticAnalyzer
{
    private Node _mainNode;
    
    public SemanticAnalyzer(Node mainNode)
    {
        if (mainNode.Type != NodeType.Program)
            throw new InvalidOperationException();
        _mainNode = mainNode;
    }

    public void Analyze()
    {
        
    }
}