using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ExpressionTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ExpressionTail;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}