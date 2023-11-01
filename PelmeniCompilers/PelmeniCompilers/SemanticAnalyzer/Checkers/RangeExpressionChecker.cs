using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RangeExpressionChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RangeExpression;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}