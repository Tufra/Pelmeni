using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ElseTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ElseTail;
    
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}