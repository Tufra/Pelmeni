using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RoutineCallChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RoutineCall;

    public override void Check(Node node)
    {
        var identifier = node.Children[0]!;
        var parameters = node.Children[1]!;

        var routine = GetRoutineOrThrowIfNotDeclared(node);

        parameters.CheckSemantic();
        var computedParams = RoutineCallParametersChecker.GetComputedParams(parameters);

        if (computedParams.Count != routine.Parameters.Count)
        {
            throw new InvalidOperationException(
                $"Routine {routine.Name} accepts {routine.Parameters.Count} arguments, but {computedParams.Count} were given at {identifier.Token!.Location}");
        }

        for (var i = 0; i < computedParams.Count; i++)
        {
            if (computedParams[i].ValueType != routine.Parameters[i].Type)
            {
                throw new InvalidOperationException(
                    $"Routine {routine.Name} accepts {routine.Parameters[i].Type} at {i + 1} position, but {computedParams[i].ValueType} provided");
            }
        }
        
    }
}