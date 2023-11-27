using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class IncrementNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Increment;
    
    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;

        var leftValue = node.Children[0]!;

        codeGeneratorContext.IsLeftValue = true;
        leftValue.GenerateCode(codeGeneratorContext);
        codeGeneratorContext.IsLeftValue = false;
        
        leftValue.GenerateCode(codeGeneratorContext);
        
        il.LoadConstantI8(1);
        il.OpCode(ILOpCode.Add);

        var varType = codeGeneratorContext.VariableType;
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
            default:
            {
                throw new InvalidOperationException($"invalid variable type {varType.ToString()}");
            }
        }
    }
}