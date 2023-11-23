using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class ReturnNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Return;
    public static MethodDefinitionHandle hanldle;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;

        var expr = node.Children[0]!;

        expr.GenerateCode(codeGeneratorContext);

        il.OpCode(ILOpCode.Ret);
    }

}