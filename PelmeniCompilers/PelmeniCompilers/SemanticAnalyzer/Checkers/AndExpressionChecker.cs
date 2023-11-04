using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class AndExpressionChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.AndExpression;

    public override void Check(Node node)
    {
        if (node.Children.Count == 1) // Relation
        {
            var subexpression = node.Children[0];
            subexpression.CheckSemantic();

            var computedSub = subexpression.BuildComputedExpression();

            node.Children = new List<Node> { computedSub };
        }
        else // AndExpression AND Relation
        {
            var oper = node.Children[0].Token!;
            var leftOperand = node.Children[1];
            var rightOperand = node.Children[2];

            leftOperand.CheckSemantic();
            rightOperand.CheckSemantic();
            
            var leftComputed = leftOperand.BuildComputedExpression();
            node.Children[1] = leftComputed;

            var rightComputed = rightOperand.BuildComputedExpression();
            node.Children[2] = rightComputed;

            var leftType = leftComputed.ValueType;
            var rightType = rightComputed.ValueType;

            if (!(leftType == "boolean" && rightType == "boolean"))
            {
                throw new InvalidOperationException(
                    $"The AND opertaion is defined only for booleans, {leftType} and {rightType} encountered at {oper.Location}"
                    );
            }

            if (leftComputed.Value == "True" && rightComputed.Value == "True")
            {
                var computed = new ComputedExpression(node.Type, null, "boolean", "True");
                node.Children = new List<Node> { computed };
            }
            else if (leftComputed.Value == "False" || rightComputed.Value == "False")
            {
                var computed = new ComputedExpression(node.Type, null, "boolean", "False");
                node.Children = new List<Node> { computed };
            }

        }
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        if (node.Children.Count == 1) // Expression
        {
            var child = (ComputedExpression)node.Children[0];
            var computed = new ComputedExpression(node.Type, child.Token, child.ValueType, child.Value)
            {
                Children = node.Children
            };
            return computed;
        }
        else // AndExpression AND Relation
        {
            var computed = new ComputedExpression(node.Type, null, "boolean", null)
            {
                Children = node.Children
            };
            return computed;
        }
    }
}