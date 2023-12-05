using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Immutable;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class AssignmentNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Assignment;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;

        var leftValue = node.Children[0]!;
        var rightValue = node.Children[1]!;

        codeGeneratorContext.IsLeftValue = true;
        leftValue.GenerateCode(codeGeneratorContext);
        
        var varType = codeGeneratorContext.VariableType;
        
        codeGeneratorContext.IsLeftValue = false;
        codeGeneratorContext.IsValueObsolete = false;
        rightValue.GenerateCode(codeGeneratorContext);
        codeGeneratorContext.IsValueObsolete = true;
        
        if (((ComputedExpression)rightValue.Children[0]).ValueType != ((ComputedExpression)leftValue).ValueType)
        {
            TypeDeclarationNodeCodeGenerator.ConvertType(il,
                ((ComputedExpression)rightValue.Children[0]).ValueType,
                ((ComputedExpression)leftValue).ValueType);
        }

        
        switch (varType)
        {
            case VariableType.Local:
            {
                il.StoreLocal(codeGeneratorContext.TokenOffset);
                break;
            }
            case VariableType.Argument:
            {
                il.StoreArgument(codeGeneratorContext.TokenOffset);
                break;
            }
            case VariableType.ArrayElement:
            {
                il.OpCode(ILOpCode.Stelem_ref);
                break;
            }
            case VariableType.Field:
            {
                il.OpCode(ILOpCode.Stfld);
                il.Token(MetadataTokens.FieldDefinitionHandle(codeGeneratorContext.TokenOffset));
                break;
            }
            case VariableType.Global:
            {
                il.OpCode(ILOpCode.Stsfld);
                il.Token(MetadataTokens.FieldDefinitionHandle(codeGeneratorContext.TokenOffset));
                break;
            }
            default:
            {
                throw new InvalidOperationException($"invalid variable type {varType.ToString()}");
            }
        }
    }
    
}