using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RelationTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RelationTail;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}