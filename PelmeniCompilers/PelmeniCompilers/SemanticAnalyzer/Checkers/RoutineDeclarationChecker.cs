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
        
        Scope.AddFrame(parameters.ToArray());
        Chain.Push(node);

        var type = node.Children[2];
        type.CheckSemantic();

        var returnType = GetReturnType(node.Children[2]);

        var routineSignature = new RoutineVirtualTableEntry()
        {
            Name = identifier,
            Parameters = parameters,
            ReturnType = returnType,
            LocalVariablesCounter = CountLocalVariables(node.Children[3])
        };
        RoutineVirtualTable[routineSignature.Name] = routineSignature;

        var body = node.Children[3];
        body.CheckSemantic();
        var computedBody = body.BuildComputedExpression();
        node.Children[3] = computedBody;

        if (type.Children.Count > 0 && computedBody.ValueType != returnType)
        {
            throw new InvalidOperationException(
                $"Routine {identifier} is supposed to return {type.Children[0].Token!.Value}, but {computedBody.ValueType} encountered");
        }
        else if (type.Children.Count == 0 && computedBody.ValueType != "None")
        {
            throw new InvalidOperationException(
                $"Routine {identifier} is not supposed to return values, but {computedBody.ValueType} encountered");
        }
        
        Scope.RemoveLastFrame();
        Chain.Pop();
    }

    private List<VariableVirtualTableEntry> GetParameters(Node parameters)
    {
        parameters.CheckSemantic();

        var result = new List<VariableVirtualTableEntry>();
        foreach (var parameterDeclaration in parameters.Children)
        {
            var identifier = GetIdentifierOrThrowIfOccupied(parameterDeclaration);
            var type = parameterDeclaration.Children[1];
            
            result.Add(ParameterDeclarationChecker.BuildVirtualTableEntry(identifier, type));
        }

        return result;
    }

    private string GetReturnType(Node tail)
    {
        if (tail.Children.Count > 0)
        {
            var type = tail.Children[0]!;
            if (type.Type == NodeType.Token)
            {
                return tail.Children[0].Token!.Value;
            }
            if (type.Type == NodeType.ArrayType)
            {
                return ArrayTypeChecker.BuildString(type);
            }
            
        }
        return "None";
        
    }

    private static int CountLocalVariables(Node node)
    {
        return node.Type == NodeType.VariableDeclaration ? 1 : node.Children.Sum(CountLocalVariables);
    }
}