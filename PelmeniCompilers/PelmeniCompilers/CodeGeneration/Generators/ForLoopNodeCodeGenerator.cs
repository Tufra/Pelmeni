using System.Reflection.Metadata;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class ForLoopNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.ForLoop;
    
    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var counterIndex = codeGeneratorContext.RoutineVirtualTableEntry.LocalVariablesCounter +
                           codeGeneratorContext.ForLoopCounter + codeGeneratorContext.ForEachLoopCounter * 2;
        codeGeneratorContext.ForLoopCounter++;

        var varEncoder = codeGeneratorContext.VarEncoder!.Value;
        var il = codeGeneratorContext.InstructionEncoder;
        
        
        varEncoder.AddVariable().Type().Int64();
        codeGeneratorContext.LocalVariablesIndex![node.Children[0].Token!.Value] = counterIndex;

        var bodyNode = node.Children[2];
        var rangeNode = node.Children[1];
        var isReverse = !rangeNode.Children[0].IsTerminal(); 
        var beginRangeNode = rangeNode.Children[1].Children[0];
        var endRangeNode = rangeNode.Children[1].Children[1];

        if (isReverse)
        {
            (beginRangeNode, endRangeNode) = (endRangeNode, beginRangeNode);
        }
        
        var condLabel = il.DefineLabel();
        var endLabel = il.DefineLabel();
        
        beginRangeNode.GenerateCode(codeGeneratorContext);
        il.StoreLocal(counterIndex);

        #region cond
        
        il.MarkLabel(condLabel);
        
        il.LoadLocal(counterIndex);
        endRangeNode.GenerateCode(codeGeneratorContext);
       
        il.Branch(!isReverse ? ILOpCode.Bge : ILOpCode.Ble, endLabel);

        #endregion

        #region body

        bodyNode.GenerateCode(codeGeneratorContext);
        
        il.LoadLocal(counterIndex);
        il.LoadConstantI8(1);
        il.OpCode(!isReverse ? ILOpCode.Add : ILOpCode.Sub);
        il.StoreLocal(counterIndex);
        
        il.Branch(ILOpCode.Br, condLabel);

        #endregion
       
       il.MarkLabel(endLabel);
    }
}