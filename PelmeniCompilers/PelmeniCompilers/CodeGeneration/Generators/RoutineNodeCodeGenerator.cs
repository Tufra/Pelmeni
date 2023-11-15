using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class RoutineNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.RoutineDeclaration;

    public override void GenerateCode(Node node, MetadataBuilder metadataBuilder, BlobBuilder ilBuilder)
    {
        var routineSignature = new BlobBuilder();

        var blobEncoder = new BlobEncoder(routineSignature);
        blobEncoder.MethodSignature().
            Parameters(node.Children[1].Children.Count, returnType => returnType.Void(), parameters => EncodeParameters(node.Children[1].Children, parameters));
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