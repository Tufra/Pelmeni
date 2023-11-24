using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class OrExpressionNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.OrExpression;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

        if (node.Children.Count == 1)
        {
            var child = node.Children[0]!;
            if (((ComputedExpression)child).Value is not null)
            {
                ExpressionNodeCodeGenerator.LoadValueFromComputed((ComputedExpression)child, il, metadata);
            }
            else
            {
                child.GenerateCode(codeGeneratorContext);
            }
        }
        else
        {
            var left = node.Children[1]!;
            var right = node.Children[2];

            if (((ComputedExpression)left).Value is not null)
            {
                ExpressionNodeCodeGenerator.LoadValueFromComputed(((ComputedExpression)left), il, metadata);
            }
            else
            {
                left.GenerateCode(codeGeneratorContext);
            }

            if (((ComputedExpression)right).Value is not null)
            {
                ExpressionNodeCodeGenerator.LoadValueFromComputed(((ComputedExpression)right), il, metadata);
            }
            else
            {
                right.GenerateCode(codeGeneratorContext);
            }

            il.OpCode(ILOpCode.Or);
        }
    }

    
}