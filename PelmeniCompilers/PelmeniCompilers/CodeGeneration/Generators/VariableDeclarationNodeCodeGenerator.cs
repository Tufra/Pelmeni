using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Immutable;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class VariableDeclarationNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.VariableDeclaration;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        if (codeGeneratorContext.VarEncoder is not null)
        {
            var il = codeGeneratorContext.InstructionEncoder;
            var varEncoder = codeGeneratorContext.VarEncoder.Value;

            var identifier = node.Children[0].Token!.Value;
            var type = node.Children[1]!;
            var initTail = node.Children[2]!;

            EncodeVariable(varEncoder,identifier, type, initTail, codeGeneratorContext);

            if (initTail.Children.Count > 0)
            {
                initTail.Children[0].GenerateCode(codeGeneratorContext);
                il.StoreLocal(codeGeneratorContext.LocalVariablesIndex![identifier]);
            }
        }
        
    }

    private void EncodeVariable(LocalVariablesEncoder varEncoder, string identifier, Node typeTail, Node expr, CodeGeneratorContext context)
    {
        if (typeTail.Children.Count == 0)
        {
            var exprType = ((ComputedExpression)expr.Children[0]!).ValueType;
            typeTail.Children = new List<Node> { new Node(NodeType.Token, new Token() { Value = exprType }) };
        }
        var type = typeTail.Children[0]!;
        if (type.Type == NodeType.Token)
        {
            switch (type.Token!.Value)
            {
                case "integer":
                {
                    varEncoder.AddVariable().Type().Int64();
                    break;
                }
                case "real":
                {
                    varEncoder.AddVariable().Type().Double();
                    break;
                }
                case "boolean":
                {
                    varEncoder.AddVariable().Type().Boolean();
                    break;
                }
                case "char":
                {
                    varEncoder.AddVariable().Type().Char();
                    break;
                }
                case "string":
                {
                    varEncoder.AddVariable().Type().String();
                    break;
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }

        }
        if (type.Type == NodeType.ArrayType)
        {
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
                {
                    EntityHandle record;
                    var success = GeneratedRecords.TryGetValue(elementType.Token!.Value, out record);
                    if (success)
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

            varEncoder.AddVariable().Type().Array(elementTypeDelegate, arrayShapeDelegate);
        }

        context.LocalVariablesIndex!.Add(identifier, context.LastVariableIndex + 1);
        context.LastVariableIndex++;
    }

    public static void EncodeField(Node node, CodeGeneratorContext context)
    {
        var identifier = node.Children[0].Token!.Value;
        var type = node.Children[1].Children[0]!;

        var fieldBuilder = new BlobBuilder();
        var fieldEncoder = new BlobEncoder(fieldBuilder).FieldSignature();

        if (type.Type == NodeType.Token)
        {
            switch (type.Token!.Value)
            {
                case "integer":
                {
                    fieldEncoder.Int64();
                    break;
                }
                case "real":
                {
                    fieldEncoder.Double();
                    break;
                }
                case "boolean":
                {
                    fieldEncoder.Boolean();
                    break;
                }
                case "char":
                {
                    fieldEncoder.Char();
                    break;
                }
                case "string":
                {
                    fieldEncoder.String();
                    break;
                }
                default:
                {
                    EntityHandle record;
                    var success = GeneratedRecords.TryGetValue(type.Token!.Value, out record);
                    if (success)
                    {
                        fieldEncoder.Type(record, false);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown type {type.Token!.Value}");
                    }
                    
                    break;
                }
            }

        }
        if (type.Type == NodeType.ArrayType)
        {
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
                {
                    EntityHandle record;
                    var success = GeneratedRecords.TryGetValue(elementType.Token!.Value, out record);
                    if (success)
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

            fieldEncoder.Array(elementTypeDelegate, arrayShapeDelegate);
        }
        
        var handle = context.MetadataBuilder.GetOrAddBlob(fieldBuilder);

        context.MetadataBuilder.AddFieldDefinition(
            System.Reflection.FieldAttributes.Public,
            context.MetadataBuilder.GetOrAddString(identifier),
            handle);

        context.LastFieldIndex++;
    }

}