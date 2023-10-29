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
    private Node _mainNode;
    
    public SemanticAnalyzer(Node mainNode)
    {
        mainNode.ThrowIfNodeNotTypeOf(NodeType.Program);
        
        _mainNode = mainNode;
    }

    public void Analyze()
    {
        //todo свернуть алиасинг, проверить семантику, оптимизация 
    }
    
   
}