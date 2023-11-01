using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ArrayAccessChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ArrayAccess;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}