using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class OperatorDeclarationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.OperatorDeclaration;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}