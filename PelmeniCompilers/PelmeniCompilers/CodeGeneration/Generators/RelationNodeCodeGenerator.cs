using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class RelationNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Relation;

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
            var op = node.Children[0].Token!.Value;
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

            switch (op)
            {
                case ">":
                {
                    il.OpCode(ILOpCode.Cgt);
                    break;
                }
                case ">=":
                {
                    il.OpCode(ILOpCode.Clt);
                    il.OpCode(ILOpCode.Not);
                    break;
                }
                case "<":
                {
                    il.OpCode(ILOpCode.Clt);
                    break;
                }
                case "<=":
                {
                    il.OpCode(ILOpCode.Cgt);
                    il.OpCode(ILOpCode.Not);
                    break;
                }
                case "=":
                {
                    il.OpCode(ILOpCode.Ceq);
                    break;
                }
                case "<>":
                {
                    il.OpCode(ILOpCode.Ceq);
                    il.OpCode(ILOpCode.Not);
                    break;
                }
            }
        }
    }

    
}