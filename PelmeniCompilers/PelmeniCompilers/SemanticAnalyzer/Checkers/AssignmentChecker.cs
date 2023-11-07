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

        if (computedPrimary.Value is not null)
        {
            if (Scope.Contains(computedPrimary.Token!.Value, 1) && computedExpr.Value is not null)
            {
                var variable = Scope.Get(computedPrimary.Token!.Value, 1)!;
                variable.Value = computedExpr.Value;
            }
            else
            {
                var variable = Scope.Get(computedPrimary.Token!.Value)!;
                variable.Value = null;
            }
        }

        if (Chain.Peek().Type is NodeType.IfStatement or NodeType.ForLoop or NodeType.WhileLoop or NodeType.ForeachLoop)
        {
            var variable = Scope.Get(computedPrimary.Token!.Value)!;
            variable.Value = null;
        }

        node.Children[0] = computedPrimary;
        node.Children[1] = computedExpr;

        if (computedPrimary.ValueType != computedExpr.ValueType)
        {
            throw new InvalidOperationException(
                $"Expected {computedPrimary.ValueType}, but {computedExpr.ValueType} encountered");
        }
    }
}