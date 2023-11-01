using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ParameterDeclarationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ParameterDeclaration;
    
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}