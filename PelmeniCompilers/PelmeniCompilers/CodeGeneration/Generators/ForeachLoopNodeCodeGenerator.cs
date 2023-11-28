using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class ForeachLoopNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.ForeachLoop;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var arrayNode = node.Children[1];
        
        var counterIndex = codeGeneratorContext.RoutineVirtualTableEntry.LocalVariablesCounter +
                           codeGeneratorContext.ForLoopCounter + codeGeneratorContext.ForEachLoopCounter * 2;
        var elemIndex = counterIndex + 1;
        codeGeneratorContext.ForEachLoopCounter++;

        var elemType = ((ComputedExpression)node.Children[1]).ValueType;

        var varEncoder = codeGeneratorContext.VarEncoder!.Value;
        var il = codeGeneratorContext.InstructionEncoder;


        varEncoder.AddVariable().Type().Int64();

        VariableDeclarationNodeCodeGenerator.EncodeVariable(varEncoder, node.Children[0].Token!.Value,
            new Node(NodeType.TypeTail,
                new List<Node>()
                {
                    new Node(NodeType.Token,
                        new Token() { Value = ArrayTypeChecker.GetElementTypeFromString(elemType) })
                }),
            new Node(NodeType.Expression, new List<Node>()), codeGeneratorContext);
        codeGeneratorContext.LocalVariablesIndex![node.Children[0].Token!.Value] = elemIndex;

        var bodyNode = node.Children[2];

        var condLabel = il.DefineLabel();
        var bodyLabel = il.DefineLabel();

        // index init
        il.LoadConstantI8(0);
        il.StoreLocal(counterIndex);
        
        il.Branch(ILOpCode.Br, condLabel);
        
        #region body

        il.MarkLabel(bodyLabel);
        
        // elem init
        arrayNode.GenerateCode(codeGeneratorContext);
        il.LoadLocal(counterIndex);
        il.OpCode(ILOpCode.Ldelem_ref);
        il.StoreLocal(elemIndex);
        
        bodyNode.GenerateCode(codeGeneratorContext);

        il.LoadLocal(counterIndex);
        il.LoadConstantI8(1);
        il.OpCode(ILOpCode.Add);
        il.StoreLocal(counterIndex);

        il.Branch(ILOpCode.Br, condLabel);
        
        il.LoadLocal(counterIndex);
        il.LoadConstantI8(1);
        il.OpCode(ILOpCode.Add);
        il.StoreLocal(counterIndex);

        #endregion
        
        #region cond

        il.MarkLabel(condLabel);
        
        il.LoadLocal(counterIndex);
        
        arrayNode.GenerateCode(codeGeneratorContext);
        il.OpCode(ILOpCode.Ldlen);
        il.Branch(ILOpCode.Blt, bodyLabel);

        #endregion

      
    }
}