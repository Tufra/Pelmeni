using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.SemanticAnalyzer;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class RoutineDeclarationNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.RoutineDeclaration;
    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var identifier = node.Children[0].Token!.Value;

        var virtualTableEntry = BaseNodeRuleChecker.RoutineVirtualTable[identifier];
        var numLocalVariables = virtualTableEntry.LocalVariablesCounter + virtualTableEntry.ForLoopsCounter + virtualTableEntry.ForeachLoopsCounter * 2;

        var localVariablesBuilder = new BlobBuilder();
        var varEncoder = new BlobEncoder(localVariablesBuilder).LocalVariableSignature(numLocalVariables);

        codeGeneratorContext.ArgumentsIndex = new Dictionary<string, int>();
        codeGeneratorContext.LastArgumentIndex = -1;
        codeGeneratorContext.LocalVariablesIndex = new Dictionary<string, int>();
        codeGeneratorContext.LastVariableIndex = 0;
        codeGeneratorContext.VarEncoder = varEncoder;
        codeGeneratorContext.RoutineVirtualTableEntry = virtualTableEntry;

        var index = codeGeneratorContext.LastRoutineIndex + 1;
        codeGeneratorContext.LastRoutineIndex++;
        
        GeneratedRoutines.Add(identifier, MetadataTokens.MethodDefinitionHandle(index));


        var routineSignature = new BlobBuilder();

        var blobEncoder = new BlobEncoder(routineSignature);
        blobEncoder.MethodSignature().
            Parameters(node.Children[1].Children.Count, 
                returnType => EncodeReturnType(node.Children[2], returnType), 
                parameters => EncodeParameters(node.Children[1].Children, parameters, codeGeneratorContext));

        var codeBuilder = new BlobBuilder();
        var flowBuilder = new ControlFlowBuilder();
        var il = new InstructionEncoder(codeBuilder, flowBuilder);
        codeGeneratorContext.InstructionEncoder = il;
        // BODY

        var body = node.Children[3]!;
        body.EncodeVariables(codeGeneratorContext);
        body.GenerateCode(codeGeneratorContext);

        if (virtualTableEntry.ReturnType == "None")
        {
            il.OpCode(ILOpCode.Ret);
        }

        // END BODY
        var localVariablesBlob = codeGeneratorContext.MetadataBuilder.GetOrAddBlob(localVariablesBuilder);
        var localVariablesSignature = codeGeneratorContext.MetadataBuilder.AddStandaloneSignature(localVariablesBlob);
        var offset = codeGeneratorContext.MethodBodyStreamEncoder.AddMethodBody(il, 256, localVariablesSignature);
        
        var methodHandle = codeGeneratorContext.MetadataBuilder.AddMethodDefinition(
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

                var size = "-1";
                if (type.Children.Count > 1)
                {
                    size = ((ComputedExpression)type.Children[1]!).Value;
                }
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
                        if (size == "-1")
                        {
                            parametersEncoder.AddParameter().Type().SZArray().Int64();
                        }
                        break;
                    }
                    case "real":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Double(); };
                        if (size == "-1")
                        {
                            parametersEncoder.AddParameter().Type().SZArray().Double();
                        }
                        break;
                    }
                    case "boolean":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Boolean(); };
                        if (size == "-1")
                        {
                            parametersEncoder.AddParameter().Type().SZArray().Boolean();
                        }
                        break;
                    }
                    case "char":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Char(); };
                        if (size == "-1")
                        {
                            parametersEncoder.AddParameter().Type().SZArray().Char();
                        }
                        break;
                    }
                    case "string":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.String(); };
                        if (size == "-1")
                        {
                            parametersEncoder.AddParameter().Type().SZArray().String();
                        }
                        break;
                    }
                    default:
                    {
                        EntityHandle record;
                        var success = GeneratedRecords.TryGetValue(elementType.Token!.Value, out record);
                        if (success)
                        {
                            elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Type(record, false); };
                            if (size != "-1")
                            {
                                parametersEncoder.AddParameter().Type().SZArray().Type(record, false);
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unknown type {elementType.Token!.Value}");
                        }
                        break;
                    }
                }

                if (size != "-1")
                {
                    parametersEncoder.AddParameter().Type().Array(elementTypeDelegate, arrayShapeDelegate);
                }
                
            }
            else
            {
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
                    default:
                    {
                        if(GeneratedRecords.TryGetValue(parameter.Children[1].Children[0].Token!.Value, out var record))
                        {
                            parametersEncoder.AddParameter().Type().Type(record, false);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unknown type {parameter.Children[1].Children[0].Token!.Value}");
                        }
                        break;
                    }
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
                {
                    EntityHandle record;
                    var success = GeneratedRecords.TryGetValue(type.Token!.Value, out record);
                    if (success)
                    {
                        encoder.Type().Type(record, false);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown type {type.Token!.Value}");
                    }
                    break;
                }
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
                {
                    if(GeneratedRecords.TryGetValue(elementType.Token!.Value, out var record))
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Type(record, false); };
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown type {elementType.Token!.Value}");
                    }
                    break;
                }
            }

            encoder.Type().Array(elementTypeDelegate, arrayShapeDelegate);

            return;
        }

        throw new NotImplementedException();

    }
}