using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ExpressionChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Expression;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}