using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class TypeTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.TypeTail;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}