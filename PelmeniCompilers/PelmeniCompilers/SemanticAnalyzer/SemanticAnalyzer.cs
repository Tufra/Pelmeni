using System.Collections.Immutable;
using System.Reflection;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer;

public class SemanticAnalyzer
{
    private readonly Node _mainNode;
    
    public SemanticAnalyzer(Node mainNode)
    {
        mainNode.ThrowIfNodeNotTypeOf(NodeType.Program);
        
        _mainNode = mainNode;
    }

    public void Analyze()
    {
        _mainNode.RemoveAliasing();
        _mainNode.CheckSemantic();
        // todo оптимизация 
    }
    
   
}