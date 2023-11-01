using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class CompoundSizeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.CompoundSize;
    
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}