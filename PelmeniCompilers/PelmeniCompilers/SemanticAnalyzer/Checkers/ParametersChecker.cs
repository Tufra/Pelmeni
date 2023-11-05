using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ParametersChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Parameters;
    
    public override void Check(Node node)
    {
        CheckChildren(node);
    }
}