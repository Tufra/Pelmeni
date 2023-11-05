using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class DecrementChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Decrement;
    public override void Check(Node node)
    {
        var primary = node.Children[0]!;
        primary.CheckSemantic();
        
        var computed = primary.BuildComputedExpression();
        if (computed.ValueType != "integer" && computed.ValueType != "real")
        {
            throw new InvalidOperationException(
                $"Increment defined only for integers and reals");
        }
    }
}