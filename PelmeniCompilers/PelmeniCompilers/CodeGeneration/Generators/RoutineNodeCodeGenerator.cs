using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class RoutineNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.RoutineDeclaration;
    public static MethodDefinitionHandle hanldle = MetadataTokens.MethodDefinitionHandle(2);

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var identifier = node.Children[0].Token!.Value;

        var routineSignature = new BlobBuilder();

        var blobEncoder = new BlobEncoder(routineSignature);
        blobEncoder.MethodSignature().
            Parameters(node.Children[1].Children.Count, returnType => EncodeReturnType(node.Children[2], returnType), parameters => EncodeParameters(node.Children[1].Children, parameters));

        var codeBuilder = new BlobBuilder();
        var flowBuilder = new ControlFlowBuilder();
        var il = new InstructionEncoder(codeBuilder, flowBuilder);
        codeGeneratorContext.InstructionEncoder = il;
        // BODY

        var body = node.Children[3]!;
        body.GenerateCode(codeGeneratorContext);

        // END BODY
        var offset = codeGeneratorContext.MethodBodyStreamEncoder.AddMethodBody(il);
        
        codeGeneratorContext.MetadataBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL,
            codeGeneratorContext.MetadataBuilder.GetOrAddString(identifier),
            codeGeneratorContext.MetadataBuilder.GetOrAddBlob(routineSignature),
            offset,
            parameterList: default(ParameterHandle));
    }

    private void EncodeParameters(List<Node> parameters, ParametersEncoder parametersEncoder)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.Children[1].Children[0].Type == NodeType.ArrayType)
            {
                return;
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

    private void EncodeReturnType(Node typeTail, ReturnTypeEncoder encoder)
    {
        if (typeTail.Children.Count == 0)
        {
            encoder.Void();
            return;
        }

        var type = typeTail.Children[0]!;
        if (type.Type == NodeType.Token)
        {
            switch (type.Token!.Value)
            {
                case "integer":
                {
                    encoder.Type().Int64();
                    break;
                }
                case "real":
                {
                    encoder.Type().Double();
                    break;
                }
                case "boolean":
                {
                    encoder.Type().Boolean();
                    break;
                }
                case "char":
                {
                    encoder.Type().Char();
                    break;
                }
                case "string":
                {
                    encoder.Type().String();
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
            return;
        }

        if (type.Type == NodeType.ArrayType)
        {
            var elementType = type.Children[0]!;
            if (elementType.Type == NodeType.ArrayType)
            {
                throw new NotImplementedException();
            }
            var size = ((ComputedExpression)type.Children[1]!).Value;
            var bb = new BlobBuilder();
            var typeEncoder = new SignatureTypeEncoder(bb);
            var shapeEncoder = new ArrayShapeEncoder(bb);

            switch (elementType.Token!.Value)
            {
                case "integer":
                {
                    encoder.Type().Array(
                            delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Int64(); }, 
                            delegate (ArrayShapeEncoder shapeEncoder) { 
                                shapeEncoder.Shape(1, new ImmutableArray<int> { int.Parse(size!) }, new ImmutableArray<int> { 0 }); });
                    break;
                }
                case "real":
                {
                    encoder.Type().Array(
                            delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Double(); }, 
                            delegate (ArrayShapeEncoder shapeEncoder) { 
                                shapeEncoder.Shape(1, new ImmutableArray<int> { int.Parse(size!) }, new ImmutableArray<int> { 0 }); });
                    break;
                }
                case "boolean":
                {
                    encoder.Type().Boolean();
                    break;
                }
                case "char":
                {
                    encoder.Type().Char();
                    break;
                }
                case "string":
                {
                    encoder.Type().String();
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }
        throw new NotImplementedException();

    }
}