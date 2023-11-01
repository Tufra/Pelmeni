using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RoutineCallParametersChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RoutineCallParameters;

    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}