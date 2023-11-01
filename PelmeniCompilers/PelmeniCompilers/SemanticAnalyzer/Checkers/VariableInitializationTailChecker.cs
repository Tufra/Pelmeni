using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class VariableInitializationTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.VariableInitializationTail;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}