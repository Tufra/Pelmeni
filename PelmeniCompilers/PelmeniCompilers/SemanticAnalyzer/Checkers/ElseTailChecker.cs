using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ElseTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ElseTail;
    
    public override void Check(Node node)
    {
        if (node.Children.Count == 1)
        {
            var body = node.Children[0];
            body.CheckSemantic();
        }
    }
}