using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.SemanticAnalyzer;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class TypeDeclarationNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.TypeDeclaration;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

        var identifier = node.Children[0].Token!.Value;
        var type = node.Children[1];

        var firstFieldIndex = codeGeneratorContext.LastFieldIndex;

        var ctor = GenerateConstructor(identifier, codeGeneratorContext);

        var typeHandle = metadata.AddTypeDefinition(
            TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoLayout,
            metadata.GetOrAddString("ConsoleApplication"),
            metadata.GetOrAddString(identifier),
            baseType: codeGeneratorContext.ObjectTypeHandle,
            fieldList: MetadataTokens.FieldDefinitionHandle(firstFieldIndex),
            methodList: ctor);

        if(BaseNodeRuleChecker.RecordVirtualTable.TryGetValue(identifier, out var record))
        {
            record.FieldOffset = codeGeneratorContext.LastFieldIndex;
            foreach (var field in type.Children)
            {
                VariableDeclarationNodeCodeGenerator.EncodeField(field, codeGeneratorContext);
            }

            GeneratedRecords.Add(identifier, typeHandle);
        }
        else
        {
            throw new InvalidOperationException($"unknown record {identifier}");
        }
        
    }

    private MethodDefinitionHandle GenerateConstructor(string identifier, CodeGeneratorContext context)
    {
        var metadataBuilder = context.MetadataBuilder;
        var systemObjectTypeRef = context.ObjectTypeHandle;
        var methodBodyStream = context.MethodBodyStreamEncoder;

        var parameterlessCtorSignature = new BlobBuilder();

        new BlobEncoder(parameterlessCtorSignature).MethodSignature(isInstanceMethod: true)
            .Parameters(0, returnType => returnType.Void(), parameters => { });

        BlobHandle parameterlessCtorBlobIndex = metadataBuilder.GetOrAddBlob(parameterlessCtorSignature);

        MemberReferenceHandle objectCtorMemberRef = metadataBuilder.AddMemberReference(
            systemObjectTypeRef,
            metadataBuilder.GetOrAddString(".ctor"),
            parameterlessCtorBlobIndex);

        var codeBuilder = new BlobBuilder();
        InstructionEncoder il;

        // Emit IL for Program::.ctor
        il = new InstructionEncoder(codeBuilder);

        // ldarg.0
        il.LoadArgument(0);

        // call instance void [mscorlib]System.Object::.ctor()
        il.Call(objectCtorMemberRef);

        // ret
        il.OpCode(ILOpCode.Ret);

        int ctorBodyOffset = methodBodyStream.AddMethodBody(il);
        codeBuilder.Clear();
        
        var index = context.LastRoutineIndex;
        context.LastRoutineIndex++;
        
        GeneratedRoutines.Add($"{identifier}.ctor", MetadataTokens.MethodDefinitionHandle(index));

        return metadataBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName,
            MethodImplAttributes.IL,
            metadataBuilder.GetOrAddString(".ctor"),
            parameterlessCtorBlobIndex,
            ctorBodyOffset,
            parameterList: default(ParameterHandle));
    }

    public static void ConvertType(InstructionEncoder il, string from, string to)
    {
        switch (to)
        {
            case "string":
            {
                if (from == "integer")
                    il.Call(GeneratedRoutines["IntToString"]);
                else if (from == "real")
                    il.Call(GeneratedRoutines["RealToString"]);
                else if (from == "boolean")
                    il.Call(GeneratedRoutines["BooleanToString"]);
                else if (from == "char") 
                    il.Call(GeneratedRoutines["CharToString"]);

                break;
            }
            case "real":
            {
                il.OpCode(ILOpCode.Conv_r8);
                break;
            }
            case "integer" or "boolean":
            {
                if (from == "real") 
                    il.Call(GeneratedRoutines["RoundReal"]);

                break;
            }
        }
    }
}