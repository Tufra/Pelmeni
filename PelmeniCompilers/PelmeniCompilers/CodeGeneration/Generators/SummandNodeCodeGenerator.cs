using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class SummandNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Summand;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

        Node sign = null!;
        Node child;
        if (node.Children.Count > 1)
        {
            sign = node.Children[0]!;
            child = node.Children[1]!;
        }
        else
        {
            child = node.Children[0]!;
        }
        

        if (((ComputedExpression)child).Value is not null)
        {
            ExpressionNodeCodeGenerator.LoadValueFromComputed((ComputedExpression)child, il, metadata);
        }
        else
        {
            child.GenerateCode(codeGeneratorContext);
        }

        if (node.Children.Count > 1 && sign.Children.Count > 0)
        {
            il.OpCode(ILOpCode.Neg);
        }
    }

    
}