using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RoutineCallParametersChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RoutineCallParameters;

    public override void Check(Node node)
    {
        CheckChildren(node);

        for (var i = 0; i < node.Children.Count; i++)
        {
            var computedParam = node.Children[i].BuildComputedExpression();
            node.Children[i] = computedParam;   
        }
    }

    public static List<ComputedExpression> GetComputedParams(Node node)
    {
        var list = new List<ComputedExpression>();
        foreach (var child in node.Children)
        {
            list.Add((ComputedExpression)child);
        }

        return list;
    }
}