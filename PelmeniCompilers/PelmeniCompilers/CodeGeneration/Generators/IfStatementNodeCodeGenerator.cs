using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class IfStatementNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.IfStatement;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

        var endLabel = il.DefineLabel();
        var elseLabel = il.DefineLabel();

        var cond = node.Children[0]!;
        var thenBody = node.Children[1]!;
        var elseBody = node.Children[2]!;
        
        cond.GenerateCode(codeGeneratorContext);
        il.Branch(ILOpCode.Brfalse, elseLabel);
        
        // then
        thenBody.GenerateCode(codeGeneratorContext);
        il.Branch(ILOpCode.Br, endLabel);

        //else
        il.MarkLabel(elseLabel);
        if (elseBody.Children.Count > 0)
        {
            elseBody.Children[0].GenerateCode(codeGeneratorContext);
        }

        il.MarkLabel(endLabel);

    }

    
}