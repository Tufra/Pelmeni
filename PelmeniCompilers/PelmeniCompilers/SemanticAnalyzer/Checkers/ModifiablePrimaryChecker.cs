using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ModifiablePrimaryChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ModifiablePrimary;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}