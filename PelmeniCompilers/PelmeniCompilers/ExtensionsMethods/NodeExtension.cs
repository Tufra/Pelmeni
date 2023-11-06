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

    private static readonly Dictionary<string, Node> Aliasing;

    static NodeExtension()
    {
        SemanticRules = GetSemanticRules();
        Aliasing = new Dictionary<string, Node>();
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

    public static void RemoveAliasing(this Node node)
    {
        if (!node.TryDeclareAlias())
        {
            node.TryReplaceAlias();
        }

        foreach (var child in (node.Children ?? new List<Node>()))
        {
            child.RemoveAliasing();
        }
    }

    private static bool TryDeclareAlias(this Node node)
    {
        if (node.Type != NodeType.TypeDeclaration) return false;

        var alias = node.Children[0].Token!.Value;
        var type = node.Children[1];

        if (type.Type is NodeType.RecordType or NodeType.ArrayType or NodeType.Token)
        {
            Aliasing[alias] = type;
            return true;
        }

        return false;
    }

    private static void TryReplaceAlias(this Node node)
    {
        if (node.Type != NodeType.TypeTail) return;
        if(node.Children.Count == 0) return;

        var type = node.Children[0].Type is NodeType.ArrayType ? node.Children[0].Children[0].Token!.Value : node.Children[0].Token!.Value;

        if (Aliasing.TryGetValue(type, out var value))
            node.Children = new List<Node>() { value };
    }
}