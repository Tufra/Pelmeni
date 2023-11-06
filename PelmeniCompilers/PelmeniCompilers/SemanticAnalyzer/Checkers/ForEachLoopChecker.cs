using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ForEachLoopChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ForeachLoop;
    public override void Check(Node node)
    {
        var iter = node.Children[0]!;
        var iterName = GetIdentifierOrThrowIfOccupied(node);

        var primary = node.Children[1]!;
        primary.CheckSemantic();
        var computedPrimary = primary.BuildComputedExpression();

        if (!TypeDeclarationChecker.IsArrayType(computedPrimary.ValueType))
        {
            throw new InvalidOperationException(
                $"Foreach is possible only over arrays, but {computedPrimary.ValueType} encountered at {computedPrimary.Token!.Location}");
        }

        var iterEntry = new VariableVirtualTableEntry()
        {
            Name = iterName,
            Type = ArrayTypeChecker.GetElementTypeFromString(computedPrimary.ValueType)
        };

        Scope.AddFrame(iterEntry);
        Chain.Push(node);

        node.Children[1] = computedPrimary;

        var body = node.Children[2]!;
        body.CheckSemantic();

        var computedBody = body.BuildComputedExpression();
        node.Children[2] = computedBody;

        Scope.RemoveLastFrame();
        Chain.Pop();
    }
}