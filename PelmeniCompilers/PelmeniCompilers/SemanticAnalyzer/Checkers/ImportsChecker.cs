using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ImportsChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Imports;
    public override void Check(Node node)
    {
        
    }
    
    
}