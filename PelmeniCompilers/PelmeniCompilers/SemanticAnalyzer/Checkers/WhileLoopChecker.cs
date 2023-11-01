using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class WhileLoopChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.WhileLoop;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}