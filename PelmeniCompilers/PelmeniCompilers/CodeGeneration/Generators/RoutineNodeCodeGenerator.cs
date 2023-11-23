using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class RoutineNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.RoutineDeclaration;
    public static MethodDefinitionHandle hanldle;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var routineSignature = new BlobBuilder();

        var blobEncoder = new BlobEncoder(routineSignature);
        blobEncoder.MethodSignature().
            Parameters(node.Children[1].Children.Count, returnType => returnType.Void(), parameters => EncodeParameters(node.Children[1].Children, parameters));

        var codeBuilder = new BlobBuilder();
        var flowBuilder = new ControlFlowBuilder();
        var il = new InstructionEncoder(codeBuilder, flowBuilder);
        MethodDefinitionHandle mainMethodDef = default;
        
        
        il.Call(mainMethodDef);
        il.OpCode(ILOpCode.Ret);
        var offset = codeGeneratorContext.MethodBodyStreamEncoder.AddMethodBody(il);
        
        mainMethodDef = codeGeneratorContext.MetadataBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL,
            codeGeneratorContext.MetadataBuilder.GetOrAddString(Guid.NewGuid().ToString()),
            codeGeneratorContext.MetadataBuilder.GetOrAddBlob(routineSignature),
            offset,
            parameterList: default(ParameterHandle));
        hanldle = mainMethodDef;
    }

    private void EncodeParameters(List<Node> parameters, ParametersEncoder parametersEncoder)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.Children[1].Children[0].Type == NodeType.ArrayType)
            {
                continue;
            }

            if (BaseNodeCodeGenerator.GeneratedRecords.ContainsKey(parameter.Children[1].Children[0].Token!.Value))
            {
                continue;
            }

            switch (parameter.Children[1].Children[0].Token!.Value)
            {
                case "real":
                {
                    parametersEncoder.AddParameter().Type().Double();
                    break;
                }
                case "integer":
                {
                    parametersEncoder.AddParameter().Type().Int64();
                    break;
                }
                case "boolean":
                {
                    parametersEncoder.AddParameter().Type().Boolean();
                    break;
                }
                case "char":
                {
                    parametersEncoder.AddParameter().Type().Char();
                    break;
                }
                case "string":
                {
                    parametersEncoder.AddParameter().Type().String();
                    break;
                }
            }
        }
    }
}