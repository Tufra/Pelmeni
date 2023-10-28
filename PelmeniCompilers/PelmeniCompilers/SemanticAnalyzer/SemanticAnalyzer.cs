using System.Collections.Immutable;
using System.Reflection;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.Rules;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer;

public class SemanticAnalyzer
{
    private Node _mainNode;
    public IReadOnlyCollection<BaseSemanticRule> SemanticRules { get; private set; }
    
    public SemanticAnalyzer(Node mainNode)
    {
        mainNode.ThrowIfNodeNotTypeOf(NodeType.Program);
        
        _mainNode = mainNode;
        SemanticRules = GetRules();
    }

    public void Analyze()
    {
        
    }
    
    private IReadOnlyCollection<BaseSemanticRule> GetRules()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var baseClassType = typeof(BaseSemanticRule);
        var derivedTypes = types.Where(t => baseClassType.IsAssignableFrom(t) && t != baseClassType)
            .Select(t => (BaseSemanticRule)Activator.CreateInstance(t)!)
            .ToHashSet();
        return derivedTypes;
    }
}