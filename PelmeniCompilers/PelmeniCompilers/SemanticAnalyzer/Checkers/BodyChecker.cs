using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class BodyChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Body;
    
    public override void Check(Node node)
    {
        CheckChildren(node);
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        var statements = node.Children;
        foreach (var child in statements)
        {
            if (child.Type == NodeType.Return)
            {
                var computedReturn = child.BuildComputedExpression();
                var computed = new ComputedExpression(node.Type, null, computedReturn.ValueType, computedReturn.Value);
                return computed;
            }

            // TODO if return in loop or condition
        }

        return new ComputedExpression(node.Type, null, "None", null);
    }
    
}