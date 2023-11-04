using System.Collections;
using System.Reflection;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeExtension
{
    private static readonly IReadOnlyCollection<BaseNodeRuleChecker> SemanticRules;

    static NodeExtension()
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

    public static void ThrowIfNodeNotTypeOf(this Node node, NodeType type)
    {
        if (node.Type != type)
            throw new InvalidOperationException($"{node} is expected to be {type}");
    }
}