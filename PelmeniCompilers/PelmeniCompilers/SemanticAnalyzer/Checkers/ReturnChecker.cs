using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ReturnChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Return;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}