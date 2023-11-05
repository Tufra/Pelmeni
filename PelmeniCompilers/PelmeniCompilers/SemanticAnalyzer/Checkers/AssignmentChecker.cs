using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class AssignmentChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Assignment;
    public override void Check(Node node)
    {
        var primary = node.Children[0]!;
        var expr = node.Children[1]!;

        primary.CheckSemantic();
        expr.CheckSemantic();

        var computedPrimary = primary.BuildComputedExpression();
        var computedExpr = expr.BuildComputedExpression();

        node.Children[0] = computedPrimary;
        node.Children[1] = computedExpr;

        if (computedPrimary.ValueType != computedExpr.ValueType)
        {
            throw new InvalidOperationException(
                $"Expected {computedPrimary.ValueType}, but {computedExpr.ValueType} encountered");
        }
    }
}