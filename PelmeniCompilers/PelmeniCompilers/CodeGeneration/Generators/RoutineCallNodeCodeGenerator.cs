using System.Reflection.Metadata;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class RoutineCallNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.RoutineCall;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

        var identifier = node.Children[0].Token!.Value;
        var args = node.Children[1]!;

        var virtualTableEntry = BaseNodeRuleChecker.GetRoutineOrThrowIfNotDeclared(node);
        
        for (var index = 0; index < args.Children.Count; index++)
        {
            var param = args.Children[index];
            
            param.GenerateCode(codeGeneratorContext);
           
            if (((ComputedExpression)param).ValueType != virtualTableEntry.Parameters[index].Type)
            {
                TypeDeclarationNodeCodeGenerator.ConvertType(il,
                    ((ComputedExpression)param).ValueType,
                    virtualTableEntry.Parameters[index].Type);
            }
        }

        var routine = BaseNodeRuleChecker.GetRoutineOrThrowIfNotDeclared(node);

        var handle = GeneratedRoutines[identifier];
        il.Call(handle);
        if (Chain.Peek().Type == NodeType.Body && routine.ReturnType != "None")
        {
            il.OpCode(ILOpCode.Pop);
        }
    }
}