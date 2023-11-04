using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RoutineDeclarationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RoutineDeclaration;

    public override void Check(Node node)
    {
        var identifier = GetIdentifierOrThrowIfOccupied(node);
        var parameters = GetParameters(node.Children[1]);
        var returnType = GetReturnType(node.Children[2]);

        var routineSignature = new RoutineVirtualTableEntry()
        {
            Name = identifier,
            Parameters = parameters,
            ReturnType = returnType
        };

        RoutineVirtualTable[routineSignature.Name] = routineSignature;
        Scope.AddFrame(parameters.ToArray());
        Chain.Push(node);

        var body = node.Children[3];
        body.CheckSemantic();
        
        Scope.RemoveLastFrame();
        Chain.Pop();
    }

    private List<VariableVirtualTableEntry> GetParameters(Node parameters)
    {
        var result = new List<VariableVirtualTableEntry>();
        foreach (var parameterDeclaration in parameters.Children)
        {
            var identifier = GetIdentifierOrThrowIfOccupied(parameterDeclaration);
            var type = parameterDeclaration.Children[1].Token!.Value;

            result.Add(new VariableVirtualTableEntry()
            {
                Name = identifier,
                Type = type
            });
        }

        return result;
    }

    private string GetReturnType(Node tail)
    {
        if (tail.Children.Count > 0)
        {
            return tail.Children[0].Token!.Value;
        }
        return "";
        
    }
}