using System.Reflection;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeSemanticExtenstion
{
    private static readonly IReadOnlyCollection<BaseNodeRuleChecker> SemanticRules;
    
    static NodeSemanticExtenstion()
    {
        SemanticRules = GetSemanticRules();
    }
    
    private static IReadOnlyCollection<BaseNodeRuleChecker> GetSemanticRules()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var baseClassType = typeof(BaseNodeRuleChecker);
        var derivedTypes = types.Where(t => baseClassType.IsAssignableFrom(t) && t != baseClassType)
            .Select(t => (BaseNodeRuleChecker)Activator.CreateInstance(t)!)
            .ToHashSet();

        return derivedTypes;
    }
    
    public static void CheckSemantic(this Node node)
    {
        var rule = GetSemanticRule(node);
        rule.Check(node);
    }
    
    public static ComputedExpression BuildComputedExpression(this Node node)
    {
        var rule = GetSemanticRule(node);
        return rule.BuildComputedExpression(node);
    }
    
    private static BaseNodeRuleChecker GetSemanticRule(Node node)
    {
        return SemanticRules.First(rule => rule.CheckingNodeType == node.Type);
    }
}