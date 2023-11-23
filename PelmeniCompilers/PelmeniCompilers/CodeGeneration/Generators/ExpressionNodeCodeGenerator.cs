using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class ExpressionNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Expression;
    public static MethodDefinitionHandle hanldle;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

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

            return;
        }

        var child = node.Children[0]!;

        child.GenerateCode(codeGeneratorContext);
    }

}