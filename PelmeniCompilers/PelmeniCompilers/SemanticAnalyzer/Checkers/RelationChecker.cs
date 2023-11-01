using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RelationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Relation;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}