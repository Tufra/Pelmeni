using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class CompoundSizeTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.CompoundSizeTail;
    
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}