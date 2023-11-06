using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ForLoopChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ForLoop;
    public override void Check(Node node)
    {
        var range = node.Children[1]!;
        var body = node.Children[2]!;

        var iterName = GetIdentifierOrThrowIfOccupied(node);
        var iterEntry = new VariableVirtualTableEntry()
        {
            Name = iterName,
            Type = "integer"
        };

        Scope.AddFrame(iterEntry);
        Chain.Push(node);

        range.CheckSemantic();
        var computedRange = range.BuildComputedExpression();
        node.Children[1] = computedRange;

        body.CheckSemantic();
        var computedBody = body.BuildComputedExpression();
        node.Children[2] = computedBody;

        Scope.RemoveLastFrame();
        Chain.Pop();
    }
}