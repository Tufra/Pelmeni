using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ModifiablePrimaryChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.ModifiablePrimary;
    public override void Check(Node node)
    {
        var chain = node.Children!;
        var variable = GetVariableOrThrowIfNotDeclared(chain[0]);

        string type = variable.Type;
        chain[0] = new ComputedExpression(chain[0].Type, chain[0].Token, type, null);

        if (variable.Node is not null)
            NodeOptimizationExtension.VariableUsage[variable.Node] = true;
        
        if (Chain.Peek().Type == NodeType.RoutineCall) // if passing to a function
        {
            ((ComputedExpression)chain[0]).Value = null;
        }
        
        for (var i = 1; i < chain.Count; i++)
        {
            if (chain[i].Type == NodeType.MemberAccess)
            {
                var memberIdentifier = chain[i].Children[0].Token!.Value;
                var location = chain[i].Children[0].Token!.Location;
                
                bool memberFound = false;

                // if primitive type
                if (TypeDeclarationChecker.IsPrimitiveType(type))
                {
                    throw new InvalidOperationException($"Type {type} does not have member {memberIdentifier} at {chain[i].Children[0].Token!.Location}");
                }

                // if array
                if (TypeDeclarationChecker.IsArrayType(type))
                {
                    if (memberIdentifier == "length")
                    {
                        type = "integer";
                        memberFound = true;
                    }
                }
                else
                {
                    // if record type
                    var record = GetRecordOrThrowIfNotDeclared(type, location);
                    foreach (var member in record.Members)
                    {
                        if (member.Name == memberIdentifier)
                        {
                            type = member.Type;
                            memberFound = true;
                            break;
                        }
                    }
                }

                

                if (!memberFound)
                {
                    throw new InvalidOperationException($"Type {type} does not have member {memberIdentifier} at {chain[i].Children[0].Token!.Location}");
                }

                var computed = new ComputedExpression(chain[i].Type, chain[i].Token, type, null)
                {
                    Children = chain[i].Children
                };
                chain[i] = computed;
                
            }
            else if (chain[i].Type == NodeType.ArrayAccess)
            {
                var index = chain[i].Children[0]!;
                index.CheckSemantic();
                var computedExpr = index.BuildComputedExpression();
                chain[i].Children[0] = computedExpr;

                if (computedExpr.ValueType != "integer")
                {
                    throw new InvalidOperationException(
                        $"Array elements index must be integer, but {computedExpr.ValueType} encountered");
                }

                var size = ArrayTypeChecker.GetArraySizeFromString(type);
                if (size != 0 && computedExpr.Value is not null && int.Parse(computedExpr.Value) > size)
                {
                    throw new InvalidOperationException(
                        $"Array elements index out of range, size is {size}, but {computedExpr.Value} accessed");
                }

                var elementType = ArrayTypeChecker.GetElementTypeFromString(type);
                var computed = new ComputedExpression(chain[i].Type, null, elementType, null)
                {
                    Children = chain[i].Children
                };
                chain[i] = computed;
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
        var computed = new ComputedExpression(node.Type, last.Token, last.ValueType, last.Value)
        {
            Children = children
        };

        return computed;
    }
}