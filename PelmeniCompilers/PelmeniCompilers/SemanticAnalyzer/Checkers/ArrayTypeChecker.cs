using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ArrayTypeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ArrayType;
    
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}