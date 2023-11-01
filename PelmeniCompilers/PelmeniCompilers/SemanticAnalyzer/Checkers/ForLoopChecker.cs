using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ForLoopChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ForLoop;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}