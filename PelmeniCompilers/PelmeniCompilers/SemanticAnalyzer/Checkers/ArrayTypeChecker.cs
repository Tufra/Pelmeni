using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ArrayTypeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ArrayType;
    
    public override void Check(Node node)
    {

        var sizeExpr = node.Children[1]!;
        var type = node.Children[0];

        if (type.Type == NodeType.Token)
        {
            var typeStr = type.Token!.Value;
            var location = type.Token!.Location;
            var isPrimitive = TypeDeclarationChecker.IsPrimitiveType(typeStr);
            if (! isPrimitive)
            {
                GetRecordOrThrowIfNotDeclared(typeStr, location);
            }
        }
        else if (type.Type == NodeType.ArrayType)
        {
            type.CheckSemantic();
        }
        else
        {
            throw new InvalidOperationException(
                $"Illegal array element type {type.Type}");
        }

        sizeExpr.CheckSemantic();
        var computedSize = sizeExpr.BuildComputedExpression();

        if (computedSize.ValueType != "integer")
        {
            throw new InvalidOperationException(
                $"Array size must be integer, {computedSize.ValueType} encountered");
        }
        if (computedSize.Value == null)
        {
            throw new InvalidOperationException(
                $"Array size must be known in compile-time");
        }

        node.Children[1] = computedSize;
    }

    public static string BuildString(Node node)
    {
        var size = ((ComputedExpression)node.Children[1]).Value;
        var type = node.Children[0];

        string? typeStr;
        if (type.Type == NodeType.Token)
        {
            typeStr = type.Token!.Value;
        }
        else if (type.Type == NodeType.ArrayType)
        {
            typeStr = BuildString(type);
        }
        else
        {
            throw new InvalidOperationException(
                $"Illegal array element type {type.Type}");
        }

        return $"array [{size}] {typeStr}";
    }
}