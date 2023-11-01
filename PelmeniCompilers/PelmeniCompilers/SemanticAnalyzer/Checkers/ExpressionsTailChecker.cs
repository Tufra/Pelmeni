using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ExpressionsTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ExpressionsTail;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}