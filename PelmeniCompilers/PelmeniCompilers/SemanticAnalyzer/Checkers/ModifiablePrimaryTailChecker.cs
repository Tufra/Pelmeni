using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ModifiablePrimaryTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ModifiablePrimaryTail;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}