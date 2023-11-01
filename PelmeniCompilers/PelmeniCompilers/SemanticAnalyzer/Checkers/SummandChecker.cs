using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class SummandChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Summand;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}