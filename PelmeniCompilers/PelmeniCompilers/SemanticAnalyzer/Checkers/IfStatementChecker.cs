using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class IfStatementChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.IfStatement;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}