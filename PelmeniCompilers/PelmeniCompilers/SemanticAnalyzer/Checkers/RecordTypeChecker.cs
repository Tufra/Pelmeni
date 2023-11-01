using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RecordTypeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RecordType;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}