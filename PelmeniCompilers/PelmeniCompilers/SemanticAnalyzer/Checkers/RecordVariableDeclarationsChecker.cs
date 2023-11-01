using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RecordVariableDeclarationsChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RecordVariableDeclarations;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}