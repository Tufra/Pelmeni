using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ReverseChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Reverse;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}