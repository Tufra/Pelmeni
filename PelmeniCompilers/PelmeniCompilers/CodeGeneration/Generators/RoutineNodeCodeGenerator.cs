using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.SemanticAnalyzer;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class RoutineNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.RoutineDeclaration;
    public static MethodDefinitionHandle hanldle = MetadataTokens.MethodDefinitionHandle(2);

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var identifier = node.Children[0].Token!.Value;

        var numLocalVariables = BaseNodeRuleChecker.RoutineVirtualTable[identifier].LocalVariablesCounter;

        var localVariablesBuilder = new BlobBuilder();
        var varEncoder = new BlobEncoder(localVariablesBuilder).LocalVariableSignature(numLocalVariables);

        codeGeneratorContext.ArgumentsIndex = new Dictionary<string, int>();
        codeGeneratorContext.LastArgumentIndex = -1;
        codeGeneratorContext.LocalVariablesIndex = new Dictionary<string, int>();
        codeGeneratorContext.LastVariableIndex = -1;
        codeGeneratorContext.VarEncoder = varEncoder;


        var routineSignature = new BlobBuilder();

        var blobEncoder = new BlobEncoder(routineSignature);
        blobEncoder.MethodSignature().
            Parameters(node.Children[1].Children.Count, returnType => EncodeReturnType(node.Children[2], returnType), parameters => EncodeParameters(node.Children[1].Children, parameters, codeGeneratorContext));

        var codeBuilder = new BlobBuilder();
        var flowBuilder = new ControlFlowBuilder();
        var il = new InstructionEncoder(codeBuilder, flowBuilder);
        codeGeneratorContext.InstructionEncoder = il;
        // BODY

        var body = node.Children[3]!;
        body.GenerateCode(codeGeneratorContext);

        // END BODY
        var localVariablesBlob = codeGeneratorContext.MetadataBuilder.GetOrAddBlob(localVariablesBuilder);
        var localVariablesSignature = codeGeneratorContext.MetadataBuilder.AddStandaloneSignature(localVariablesBlob);
        var offset = codeGeneratorContext.MethodBodyStreamEncoder.AddMethodBody(il, 256, localVariablesSignature);
        
        codeGeneratorContext.MetadataBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL,
            codeGeneratorContext.MetadataBuilder.GetOrAddString(identifier),
            codeGeneratorContext.MetadataBuilder.GetOrAddBlob(routineSignature),
            offset,
            parameterList: default(ParameterHandle));
    }

    private void EncodeParameters(List<Node> parameters, ParametersEncoder parametersEncoder, CodeGeneratorContext context)
    {
        foreach (var parameter in parameters)
        {
            var identifier = parameter.Children[0].Token!.Value;
            if (parameter.Children[1].Children[0].Type == NodeType.ArrayType)
            {
                var type = parameter.Children[1].Children[0];
                var elementType = type.Children[0]!;

                var size = ((ComputedExpression)type.Children[1]!).Value;
                var bb = new BlobBuilder();
                var typeEncoder = new SignatureTypeEncoder(bb);
                var shapeEncoder = new ArrayShapeEncoder(bb);

                Action<SignatureTypeEncoder> elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.VoidPointer(); };
                Action<ArrayShapeEncoder> arrayShapeDelegate = delegate (ArrayShapeEncoder shapeEncoder) {
                    var sizes = new List<int> { int.Parse(size!) };
                    var bounds = new List<int> { 0 };
                    shapeEncoder.Shape(1, sizes.ToImmutableArray(), bounds.ToImmutableArray()); };

                if (elementType.Type == NodeType.ArrayType)
                {
                    throw new NotImplementedException();
                }

                switch (elementType.Token!.Value)
                {
                    case "integer":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Int64(); };
                        break;
                    }
                    case "real":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Double(); };
                        break;
                    }
                    case "boolean":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Boolean(); };
                        break;
                    }
                    case "char":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Char(); };
                        break;
                    }
                    case "string":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.String(); };
                        break;
                    }
                    default:
                        throw new NotImplementedException();
                }

                parametersEncoder.AddParameter().Type().Array(elementTypeDelegate, arrayShapeDelegate);
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

            context.ArgumentsIndex!.Add(identifier, context.LastArgumentIndex + 1);
            context.LastArgumentIndex++;
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

            var size = ((ComputedExpression)type.Children[1]!).Value;
            // var bb = new BlobBuilder();
            // var typeEncoder = new SignatureTypeEncoder(bb);
            // var shapeEncoder = new ArrayShapeEncoder(bb);

            Action<SignatureTypeEncoder> elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.VoidPointer(); };
            Action<ArrayShapeEncoder> arrayShapeDelegate = delegate (ArrayShapeEncoder shapeEncoder) {
                var sizes = new List<int> { int.Parse(size!) };
                var bounds = new List<int> { 0 };
                shapeEncoder.Shape(1, sizes.ToImmutableArray(), bounds.ToImmutableArray()); };

            if (elementType.Type == NodeType.ArrayType)
            {
                throw new NotImplementedException();
            }

            switch (elementType.Token!.Value)
            {
                case "integer":
                {
                    elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Int64(); };
                    break;
                }
                case "real":
                {
                    elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Double(); };
                    break;
                }
                case "boolean":
                {
                    elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Boolean(); };
                    break;
                }
                case "char":
                {
                    elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Char(); };
                    break;
                }
                case "string":
                {
                    elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.String(); };
                    break;
                }
                default:
                    throw new NotImplementedException();
            }

            encoder.Type().Array(elementTypeDelegate, arrayShapeDelegate);

            return;
        }

        throw new NotImplementedException();

    }
}