using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class SimpleTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.SimpleTail;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}