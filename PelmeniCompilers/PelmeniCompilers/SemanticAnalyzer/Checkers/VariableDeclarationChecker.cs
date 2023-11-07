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

        Scope.AddToLastFrame(BuildVirtualTableEntry(node));
    }

    private static string BuildTypeString(Node node)
    {
        if (node.Type == NodeType.Token)
        {
            return node.Token!.Value;
        }
        else if (node.Type == NodeType.ArrayType)
        {
            return ArrayTypeChecker.BuildString(node);
        }
        else
        {
            throw new InvalidOperationException(
                    $"Illegal type {node.Type}");
        }
    }

    public static VariableVirtualTableEntry BuildVirtualTableEntry(Node node)
    {
        var identifier = GetIdentifierOrThrowIfOccupied(node);
        var type = node.Children[1]!;
        var init = node.Children[2]!;

        var variableSignature = new VariableVirtualTableEntry
        {
            Name = identifier
        };

        if (type.Children.Count == 0) // no type, init
        {
            var computedInit = init.BuildComputedExpression();
            node.Children[2] = computedInit;
            
            variableSignature.Type = computedInit.ValueType;
            variableSignature.Value = computedInit.Value!;
        }
        else if (init.Children.Count == 0) // type, no init
        {
            variableSignature.Type = BuildTypeString(type.Children[0]);
        }
        else // type, init
        {
            var computedInit = init.BuildComputedExpression();
            variableSignature.Type = computedInit.ValueType;
            variableSignature.Value = computedInit.Value!;
            
            if (computedInit.ValueType != type.Children[0].Token!.Value)
            {
                throw new InvalidOperationException(
                    $"Variable and value should have the same type, {type.Children[0].Token!.Value} and {computedInit.ValueType} encountered at {node.Children[0].Token!.Location}");
            }
            
            node.Children[2] = computedInit;
        }

        NodeOptimizationExtension.VariableUsage[node] = false;
        
        return variableSignature;
    }
}