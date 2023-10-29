using System.Collections.Immutable;
using System.Reflection;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer;

public class SemanticAnalyzer
{
    private Node _mainNode;
    public IReadOnlyCollection<BaseNodeRuleChecker> SemanticRules { get; private set; }
    
    public SemanticAnalyzer(Node mainNode)
    {
        mainNode.ThrowIfNodeNotTypeOf(NodeType.Program);
        
        _mainNode = mainNode;
        SemanticRules = GetRules();
    }

    public void Analyze()
    {
        //todo свернуть алиасинг, проверить семантику, оптимизация 
    }
    
    private IReadOnlyCollection<BaseNodeRuleChecker> GetRules()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var baseClassType = typeof(BaseNodeRuleChecker);
        var derivedTypes = types.Where(t => baseClassType.IsAssignableFrom(t) && t != baseClassType)
            .Select(t => (BaseNodeRuleChecker)Activator.CreateInstance(t)!)
            .ToHashSet();
        return derivedTypes;
    }
}