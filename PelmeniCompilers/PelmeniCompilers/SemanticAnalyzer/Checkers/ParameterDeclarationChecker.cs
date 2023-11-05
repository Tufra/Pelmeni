using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ParameterDeclarationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ParameterDeclaration;
    
    public override void Check(Node node)
    {
        var identifier = GetIdentifierOrThrowIfOccupied(node);
        var type = node.Children[1]!;

        type.CheckSemantic();

        if (type.Children.Count == 0)
        {
            throw new InvalidOperationException(
                $"Parameters must have their types specified at {node.Children[0].Token!.Location}");
        }

        // Scope.AddToLastFrame(BuildVirtualTableEntry(identifier, type));
    }

    public static VariableVirtualTableEntry BuildVirtualTableEntry(string name, Node typeTail)
    {
        var typeStr = "";
        var type = typeTail.Children[0]!;
        if (type.Type == NodeType.Token)
        {
            typeStr = type.Token!.Value;
        }
        else if (type.Type == NodeType.ArrayType)
        {
            typeStr = ArrayTypeChecker.BuildString(type);
        }
        else
        {
            throw new InvalidOperationException(
                $"Illegal type {type.Type}");
        }

        var entry = new VariableVirtualTableEntry()
        {
            Name = name,
            Type = typeStr
        };

        return entry;
    }
}