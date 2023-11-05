using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RangeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Range;
    
    public override void Check(Node node)
    {
        var rangeExpression = node.Children[1]!;
        rangeExpression.CheckSemantic();

        node.Children[1] = rangeExpression.BuildComputedExpression();
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        var computedSub = (ComputedExpression)node.Children[1]!;
        var computed = new ComputedExpression(node.Type, null, "integer", computedSub.Value)
        {
            Children = node.Children
        };
        return computed;
    }
}