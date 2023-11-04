using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ExpressionChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Expression;
    public override void Check(Node node)
    {
        var subexpression = node.Children[0]!;
        subexpression.CheckSemantic();

        var computedSub = subexpression.BuildComputedExpression();

        node.Children = new List<Node> { computedSub };
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        var child = (ComputedExpression)node.Children[0];
        var computed = new ComputedExpression(node.Type, child.Token, child.ValueType, child.Value)
        {
            Children = node.Children
        };
        return computed;
    }
}