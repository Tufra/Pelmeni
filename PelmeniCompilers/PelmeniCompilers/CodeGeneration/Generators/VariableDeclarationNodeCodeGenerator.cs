using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class VariableDeclarationNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.VariableDeclaration;
    public static MethodDefinitionHandle hanldle;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        // var il = codeGeneratorContext.InstructionEncoder;

        // var identifier = node.Children[0].Token!.Value;
        // var type = node.Children[1]!;
        // var expr = node.Children[2]!;

        // expr.GenerateCode(codeGeneratorContext);

        // il.OpCode(ILOpCode.Ret);
    }

}