using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class SimpleChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Simple;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}