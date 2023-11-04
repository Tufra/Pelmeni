using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ParametersChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Parameters;
    
    public override void Check(Node node)
    {
        var children = node.Children;

        if (children.Count > 0)
        {
            throw new NotImplementedException();
        }
    }
}