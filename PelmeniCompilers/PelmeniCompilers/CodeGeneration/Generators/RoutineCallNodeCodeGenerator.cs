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
        
        foreach (var param in args.Children)
        {
            param.GenerateCode(codeGeneratorContext);
        }

        var handle = GeneratedRoutines[identifier];
        il.Call(handle);
        if (codeGeneratorContext.IsValueObsolete && BaseNodeRuleChecker.RoutineVirtualTable[identifier].ReturnType != "None")
        {
            il.OpCode(ILOpCode.Pop);
        }
    }
}