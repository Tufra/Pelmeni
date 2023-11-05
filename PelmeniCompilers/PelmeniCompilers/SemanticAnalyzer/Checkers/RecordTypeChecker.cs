using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RecordTypeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RecordType;

    public override void Check(Node node)
    {
        CheckChildren(node);
    }

    public static List<VariableVirtualTableEntry> GetMembers(Node node)
    {
        var members = new List<VariableVirtualTableEntry>();

        foreach (var member in node.Children)
        {
            var identifier = member.Children[0].Token!.Value;
            var type = member.Children[1]!;

            if (type.Children.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Members of records must have their types specified at {member.Children[0].Token!.Location}");
            }

            var typeStr = "";
            if (type.Type == NodeType.Token)
            {
                typeStr = type.Token!.Value;
            }
            else if (type.Type == NodeType.ArrayType)
            {
                typeStr = ArrayTypeChecker.BuildString(type);
            }

            var entry = new VariableVirtualTableEntry()
            {
                Name = identifier,
                Type = typeStr
            };
            members.Add(entry);
        }

        return members;
    }
}