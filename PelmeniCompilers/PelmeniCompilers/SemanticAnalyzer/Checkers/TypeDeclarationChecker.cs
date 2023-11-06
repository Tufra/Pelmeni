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
                Members = members
            };
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
        type == "integer" || type == "real" || type == "boolean" || type == "char";

    public static bool IsArrayType(string type) =>
        type.StartsWith("array");
}