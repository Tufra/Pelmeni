using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.SemanticAnalyzer;
using PelmeniCompilers.SemanticAnalyzer.Checkers;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class ModifiablePrimaryNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.ModifiablePrimary;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;
        var isLeftValue = codeGeneratorContext.IsLeftValue;

        var children = node.Children;
        var baseIdentifier = children[0].Token!.Value;
        
        var type = ((ComputedExpression)children[0]).ValueType;
        if (codeGeneratorContext.LocalVariablesIndex!.TryGetValue(baseIdentifier, out var varIndex))
        {
            codeGeneratorContext.VariableType = VariableType.Local;
            
            var baseVarIndex = varIndex;
            if (isLeftValue && children.Count == 1)
            {
                codeGeneratorContext.TokenOffset = varIndex;
                return;
            }
            
            il.LoadLocal(baseVarIndex);

        }
        else if (codeGeneratorContext.ArgumentsIndex!.TryGetValue(baseIdentifier, out var argIndex))
        {
            codeGeneratorContext.VariableType = VariableType.Argument;
            
            var baseVarIndex = argIndex;
            if (isLeftValue && children.Count == 1)
            {
                codeGeneratorContext.TokenOffset = argIndex;
                return;
            }

            il.LoadArgument(baseVarIndex);
        }
        
        foreach (var child in children.Skip(1))
        {
            if (child.Type == NodeType.MemberAccess)
            {
                var fieldName = child.Children[0].Token!.Value;
                if (BaseNodeRuleChecker.RecordVirtualTable.TryGetValue(type, out var record))
                {
                    var fieldOffset = record.FieldOffset;
                    var fieldIndex = record.Members.FindIndex(
                        field => field.Name == fieldName);
                    if (fieldIndex != -1)
                    {
                        codeGeneratorContext.VariableType = VariableType.Field;
                        type = ((ComputedExpression)child).ValueType;
                        
                        if (child == children[^1] && isLeftValue)
                        {
                            codeGeneratorContext.TokenOffset = fieldOffset + fieldIndex;
                            break;
                        }
                        
                        il.OpCode(ILOpCode.Ldfld);
                        il.Token(MetadataTokens.FieldDefinitionHandle(fieldOffset + fieldIndex));
                    }
                    else
                    {
                        throw new InvalidOperationException($"unknown field {fieldName}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"unknown record {type}");
                }
                
            }
            else if (child.Type == NodeType.ArrayAccess)
            {
                var index = child.Children[0]!;
                index.GenerateCode(codeGeneratorContext);

                codeGeneratorContext.VariableType = VariableType.ArrayElement;
                
                
                type = ArrayTypeChecker.GetElementTypeFromString(((ComputedExpression)child).ValueType);

                if (child == children[^1] && isLeftValue)
                {
                    break;
                }
                
                il.OpCode(ILOpCode.Ldelem_ref);
            }
            else
            {
                throw new InvalidOperationException($"invalid access {child.Type}");
            }
        }
        
    }

    
}