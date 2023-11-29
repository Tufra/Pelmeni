using System.Reflection.Metadata;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class WhileLoopNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.WhileLoop;
    
    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var expressionNode = node.Children[0];
        var bodyNode = node.Children[1];
        var il = codeGeneratorContext.InstructionEncoder;

        var condLabel = il.DefineLabel();
        var endLabel = il.DefineLabel();

        #region condition
        
        il.MarkLabel(condLabel);
        codeGeneratorContext.IsValueObsolete = false;
        expressionNode.GenerateCode(codeGeneratorContext);
        codeGeneratorContext.IsValueObsolete = true;
        
        il.Branch(ILOpCode.Brfalse, endLabel);

        #endregion

        #region body

        bodyNode.GenerateCode(codeGeneratorContext);
        il.Branch(ILOpCode.Br, condLabel);
        
        #endregion
        
        il.MarkLabel(endLabel);
    }
}