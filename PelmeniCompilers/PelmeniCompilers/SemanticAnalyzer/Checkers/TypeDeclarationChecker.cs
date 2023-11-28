using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class TypeDeclarationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.TypeDeclaration;

    public override void Check(Node node)
    {
        var identifier = GetIdentifierOrThrowIfOccupied(node);
        var typeNode = node.Children[1]!;

        if (typeNode.Type == NodeType.Token)
        {
            var type = typeNode.Token!.Value;
            var location = typeNode.Token!.Location;
            if (IsPrimitiveType(type)) // if primitive alias
            {
                // TODO aliasing
                return;
            }
            // if record alias
            var record = GetRecordOrThrowIfNotDeclared(type, location);
            // TODO aliasing
        }
        else if (typeNode.Type == NodeType.RecordType) 
        {
            typeNode.CheckSemantic();
            var members = RecordTypeChecker.GetMembers(typeNode);
            var tableEntry = new RecordVirtualTableEntry()
            {
                Name = identifier,
                Members = members,
                Node = node
            };
            NodeOptimizationExtension.RecordUsage[node] = false;
            RecordVirtualTable[identifier] = tableEntry;
        }
        else // if array
        {
            typeNode.CheckSemantic();
            var typeStr = ArrayTypeChecker.BuildString(typeNode);
            // TODO aliasing
        }
    }

    public static bool IsPrimitiveType(string type) => 
        type is "integer" or "real" or "boolean" or "char" or "string";

    public static bool IsArrayType(string type) =>
        type.StartsWith("array");

    public static bool IsConvertibleTypes(string from, string? value, string to) => 
        (to is "Any") ||
        (to is "string" && IsPrimitiveType(from)) ||
        (to is "real" && from is "integer" or "boolean") ||
        (to is "integer" && from is "boolean" or "real") ||
        (to is "boolean" && (from is "integer" && value is "1" or "0" || from is "real" && value is "1.0" or "0.0"));
                    
}