using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ReturnChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Return;

    public override void Check(Node node)
    {
        var expr = node.Children[0]!;
        expr.CheckSemantic();

        var computed = expr.BuildComputedExpression();
        node.Children[0] = computed;
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