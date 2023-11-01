using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class MemberCallChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.MemberCall;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}