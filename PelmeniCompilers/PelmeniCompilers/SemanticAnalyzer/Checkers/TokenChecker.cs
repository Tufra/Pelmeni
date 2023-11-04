using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class TokenChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Token;

    public override void Check(Node node)
    {

    }

}