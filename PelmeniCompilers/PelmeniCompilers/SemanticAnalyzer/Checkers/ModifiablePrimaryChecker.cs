using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ModifiablePrimaryChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ModifiablePrimary;
    public override void Check(Node node)
    {
        var chain = node.Children!;
                
        string type = GetVariableOrThrowIfNotDeclared(chain[0]).Type;
        chain[0] = new ComputedExpression(chain[0].Type, chain[0].Token, type, null);
        for (var i = 1; i < chain.Count; i++)
        {
            if (chain[i].Type == NodeType.MemberAccess)
            {
                var memberIdentifier = chain[i].Children[0].Token!.Value;
                var location = chain[i].Children[0].Token!.Location;

                // if primitive type
                if (type == "integer" || type == "real" || type == "char" || type == "string" || type == "boolean")
                {
                    throw new InvalidOperationException($"Type {type} does not have member {memberIdentifier} at {chain[i].Children[0].Token!.Location}");
                }

                // if record type
                var record = GetRecordOrThrowIfNotDeclared(type, location);
                bool memberFound = false;
                foreach (var member in record.Members)
                {
                    if (member.Name == memberIdentifier)
                    {
                        type = member.Type;
                        memberFound = true;
                        break;
                    }
                }

                if (!memberFound)
                {
                    throw new InvalidOperationException($"Type {type} does not have member {memberIdentifier} at {chain[i].Children[0].Token!.Location}");
                }

                var computed = new ComputedExpression(chain[i].Type, chain[i].Token, type, null);
                chain[i] = computed;
                
            }
            else if (chain[i].Type == NodeType.ArrayAccess)
            {
                // TODO: if array
                throw new NotImplementedException();
            }
            
        }

        // var computed = new ComputedExpression(node.Type, null, type, null);
        // node.Children[1] = new List<Node> { computed };
        return;
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        var children = node.Children;
        var last = (ComputedExpression)children.Last();
        var computed = new ComputedExpression(node.Type, null, last.ValueType, last.Value)
        {
            Children = children
        };

        return computed;
    }
}