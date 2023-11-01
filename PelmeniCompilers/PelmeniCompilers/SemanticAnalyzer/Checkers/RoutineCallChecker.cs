using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RoutineCallChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RoutineCall;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}