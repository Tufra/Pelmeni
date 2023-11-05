using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class VariableDeclarationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.VariableDeclaration;

    public override void Check(Node node)
    {
        var identifier = GetIdentifierOrThrowIfOccupied(node);
        var type = node.Children[1]!;
        var init = node.Children[2]!;

        type.CheckSemantic();
        init.CheckSemantic();

        if (type.Children.Count == 0 && init.Children.Count == 0) // no type, no init
        {
            throw new InvalidOperationException(
                $"Variable {identifier} must have type or value specified at {node.Children[0].Token!.Location}");
        }

        var variableSignature = new VariableVirtualTableEntry
        {
            Name = identifier
        };

        if (type.Children.Count == 0) // no type, init
        {
            var computedInit = init.BuildComputedExpression();
            
            variableSignature.Type = computedInit.ValueType;
        }
        else if (init.Children.Count == 0) // type, no init
        {
            variableSignature.Type = type.Children[0].Token!.Value;
        }
        else // type, init
        {
            var computedInit = init.BuildComputedExpression();
            variableSignature.Type = computedInit.ValueType;
            
            if (computedInit.ValueType != type.Children[0].Token!.Value)
            {
                throw new InvalidOperationException(
                    $"Variable and value should have the same type, {type.Children[0].Token!.Value} and {computedInit.ValueType} encountered at {node.Children[0].Token!.Location}");
            }
        }

        Scope.AddToLastFrame(variableSignature);
    }
}