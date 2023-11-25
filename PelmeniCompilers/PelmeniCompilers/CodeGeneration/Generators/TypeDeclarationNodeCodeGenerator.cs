using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

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

        var ctor = GenerateConstructor(codeGeneratorContext);

        var typeHandle = metadata.AddTypeDefinition(
            TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoLayout,
            metadata.GetOrAddString("ConsoleApplication"),
            metadata.GetOrAddString(identifier),
            baseType: codeGeneratorContext.ObjectTypeHandle,
            fieldList: MetadataTokens.FieldDefinitionHandle(firstFieldIndex),
            methodList: ctor);

        foreach (var field in type.Children)
        {
            VariableDeclarationNodeCodeGenerator.EncodeField(field, codeGeneratorContext);
        }

        GeneratedRecords.Add(identifier, typeHandle);
    }

    private MethodDefinitionHandle GenerateConstructor(CodeGeneratorContext context)
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

        return metadataBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName,
            MethodImplAttributes.IL,
            metadataBuilder.GetOrAddString(".ctor"),
            parameterlessCtorBlobIndex,
            ctorBodyOffset,
            parameterList: default(ParameterHandle));
    }
}