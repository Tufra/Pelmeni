using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ForeachLoopChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ForeachLoop;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}