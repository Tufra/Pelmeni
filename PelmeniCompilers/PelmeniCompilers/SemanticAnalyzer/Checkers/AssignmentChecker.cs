using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class AssignmentChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Assignment;
    public override void Check(Node node)
    {
        
    }
}