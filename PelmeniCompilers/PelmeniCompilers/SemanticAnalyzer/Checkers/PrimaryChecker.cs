using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class PrimaryChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Primary;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}