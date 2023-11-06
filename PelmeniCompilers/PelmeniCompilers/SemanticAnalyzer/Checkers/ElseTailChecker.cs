using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ElseTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ElseTail;
    
    public override void Check(Node node)
    {
        if (node.Children.Count == 1)
        {
            var body = node.Children[0];
            body.CheckSemantic();
            var computedBody = body.BuildComputedExpression();
            node.Children[0] = computedBody;
        }
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        var computed = new ComputedExpression(node.Type, null, "None", null)
        {
            Children = node.Children
        };

        if (node.Children.Count == 1)
        {
            var computedBody = (ComputedExpression)node.Children[0]!;
            computed.ValueType = computedBody.ValueType;
            computed.Value = computedBody.Value;
        }

        return computed;
    }
}