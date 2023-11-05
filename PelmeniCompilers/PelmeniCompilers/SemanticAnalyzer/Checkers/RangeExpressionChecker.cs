using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RangeExpressionChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RangeExpression;

    public override void Check(Node node)
    {
        var from = node.Children[0]!;
        var to = node.Children[1]!;

        from.CheckSemantic();
        to.CheckSemantic();

        var computedFrom = from.BuildComputedExpression();
        var computedTo = to.BuildComputedExpression();

        if (computedFrom.ValueType != "integer" || computedTo.ValueType != "integer")
        {
            throw new InvalidOperationException(
                $"Range can only be constructed from integers, but {computedFrom.ValueType} and {computedTo.ValueType} encountered");
        }

        node.Children[0] = computedFrom;
        node.Children[1] = computedTo;
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        var computedFrom = (ComputedExpression)node.Children[0]!;
        var computedTo = (ComputedExpression)node.Children[1]!;
        
        if (computedFrom.Value is not null && computedTo.Value is not null)
        {
            var distance = int.Parse(computedTo.Value) - int.Parse(computedFrom.Value);
            var computed = new ComputedExpression(node.Type, null, "integer", distance.ToString())
            {
                Children = node.Children
            };
            return computed;
        }
        else
        {
            var computed = new ComputedExpression(node.Type, null, "integer", null)
            {
                Children = node.Children
            };
            return computed;
        }
    }
}