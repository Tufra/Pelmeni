using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ExpressionsChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Expressions;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}