using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RangeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Range;
    
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}