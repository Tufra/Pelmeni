using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using System.Reflection.Metadata.Ecma335;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class ExpressionNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Expression;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

        codeGeneratorContext.IsValueObsolete = false;
        
        if (node is ComputedExpression && ((ComputedExpression)node).Value is not null)
        {
            var val = ((ComputedExpression)node).Value!;
            var type = ((ComputedExpression)node).ValueType;
            switch (type)
            {
                case "integer":
                {
                    il.LoadConstantI8(int.Parse(val));
                    break;
                }
                case "real":
                {
                    il.LoadConstantR8(double.Parse(val.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture));
                    break;
                }
                case "boolean":
                {
                    if (val == "true")
                    {
                        il.LoadConstantI8(1);
                    }
                    else
                    {
                        il.LoadConstantI8(0);
                    }
                    break;
                }
                case "string" or "char":
                {
                    il.LoadString(metadata.GetOrAddUserString(val));
                    break;
                }
                default:
                {
                    throw new InvalidOperationException($"invalid computed data type {type} with value {val}");
                }
            }
            
            codeGeneratorContext.IsValueObsolete = true;
            return;
        }

        var child = node.Children[0]!;

        child.GenerateCode(codeGeneratorContext);
        
        codeGeneratorContext.IsValueObsolete = true;
    }


    public static void LoadValueFromComputed(ComputedExpression expr, InstructionEncoder il, MetadataBuilder metadata)
    {
        var type = expr.ValueType;
        var val = expr.Value;

        if (val is null)
        {
            throw new InvalidOperationException($"tried loading unknown value for {type}");
        }

        switch (type)
        {
            case "integer":
            {
                il.LoadConstantI8(int.Parse(val));
                break;
            }
            case "real":
            {
                il.LoadConstantR8(double.Parse(val));
                break;
            }
            case "boolean":
            {
                il.LoadConstantI8(val == "true"? 1 : 0);
                break;
            }
            case "char":
            {
                il.LoadConstantI8(int.Parse(val));
                break;
            }
            case "String":
            {
                il.LoadString(metadata.GetOrAddUserString(val));
                break;
            }
        }
    }
}