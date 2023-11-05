using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class VariableInitializationTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.VariableInitializationTail;

    public override void Check(Node node)
    {
        if (node.Children.Count == 1)
        {
            var expr = node.Children[0];
            expr.CheckSemantic();

            var computed = expr.BuildComputedExpression();
            node.Children[0] = computed;
        }
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        if (node.Children.Count == 1)
        {
            var expr = node.Children[0];
            var computedExp = expr.BuildComputedExpression();
            var computed = new ComputedExpression(node.Type, null, computedExp.ValueType, computedExp.Value)
            {
                Children = new List<Node> { computedExp }
            };
            return computed;
        }
        else
        {
            var computed = new ComputedExpression(node.Type, null, "None", null);
            return computed;
        }        
    }
}