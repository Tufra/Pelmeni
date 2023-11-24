using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.CodeGeneration;
using PelmeniCompilers.CodeGeneration.Generators;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class NodeCodeGenerationExtension
{
    private static readonly IReadOnlyCollection<BaseNodeCodeGenerator> CodeGenerators;

    static NodeCodeGenerationExtension()
    {
        CodeGenerators = GetCodeGenerators();
    }

    private static IReadOnlyCollection<BaseNodeCodeGenerator> GetCodeGenerators()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var baseClassType = typeof(BaseNodeCodeGenerator);
        var derivedTypes = types.Where(t => baseClassType.IsAssignableFrom(t) && t != baseClassType)
            .Select(t => (BaseNodeCodeGenerator)Activator.CreateInstance(t)!)
            .ToHashSet();

        return derivedTypes;
    }

    private static BaseNodeCodeGenerator GetCodeGenerator(Node node)
    {
        return CodeGenerators.First(rule => rule.GeneratingCodeNodeType == node.Type);
    }

    public static (MetadataBuilder metadataBuilder, BlobBuilder ilBuilder, MethodDefinitionHandle entryPoint)
        GenerateProgram(this Node node, string entryMethodName)
    {
        if (node.Type != NodeType.Program)
            throw new InvalidOperationException();

        var ilBuilder = new BlobBuilder();
        var metadataBuilder = new MetadataBuilder();
        var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);

        var codeGenerationContext = new CodeGeneratorContext()
        {
            MetadataBuilder = metadataBuilder,
            IlBuilder = ilBuilder,
            MethodBodyStreamEncoder = methodBodyStream
        };


        metadataBuilder.AddModule(
            0,
            metadataBuilder.GetOrAddString("ConsoleApplication.exe"),
            metadataBuilder.GetOrAddGuid(CodeGenerator.SGuid),
            default(GuidHandle),
            default(GuidHandle));

        metadataBuilder.AddAssembly(
            metadataBuilder.GetOrAddString("ConsoleApplication"),
            version: new Version(1, 0, 0, 0),
            culture: default(StringHandle),
            publicKey: default(BlobHandle),
            flags: 0,
            hashAlgorithm: AssemblyHashAlgorithm.None);

        var mscorlibAssemblyRef = metadataBuilder.AddAssemblyReference(
            name: metadataBuilder.GetOrAddString("mscorlib"),
            version: new Version(7, 0, 0, 0),
            culture: default(StringHandle),
            publicKeyOrToken: metadataBuilder.GetOrAddBlob(
                new byte[] { 0xB7, 0x7A, 0x5C, 0x56, 0x19, 0x34, 0xE0, 0x89 }
            ),
            flags: default(AssemblyFlags),
            hashValue: default(BlobHandle));

        var systemObjectTypeRef = metadataBuilder.AddTypeReference(
            mscorlibAssemblyRef,
            metadataBuilder.GetOrAddString("System"),
            metadataBuilder.GetOrAddString("Object"));

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

        var mainMethodDef = GenerateEntryPoint(metadataBuilder, ilBuilder, mscorlibAssemblyRef, objectCtorMemberRef, methodBodyStream);
        
        foreach (var child in node.Children)
        {
            child.GenerateCode(codeGenerationContext);
        }
        
        metadataBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName,
            MethodImplAttributes.IL,
            metadataBuilder.GetOrAddString(".ctor"),
            parameterlessCtorBlobIndex,
            ctorBodyOffset,
            parameterList: default(ParameterHandle));
        
        metadataBuilder.AddTypeDefinition(
            default(TypeAttributes),
            default(StringHandle),
            metadataBuilder.GetOrAddString("<Module>"),
            baseType: default(EntityHandle),
            fieldList: MetadataTokens.FieldDefinitionHandle(1),
            methodList: mainMethodDef);

        // Create type definition for ConsoleApplication.Program
        metadataBuilder.AddTypeDefinition(
            TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit,
            metadataBuilder.GetOrAddString("ConsoleApplication"),
            metadataBuilder.GetOrAddString("Program"),
            baseType: systemObjectTypeRef,
            fieldList: MetadataTokens.FieldDefinitionHandle(1),
            methodList: mainMethodDef);
        
        
        return (metadataBuilder, ilBuilder, mainMethodDef);
    }

    private static MethodDefinitionHandle GenerateEntryPoint(MetadataBuilder metadata, BlobBuilder ilBuilder,
        AssemblyReferenceHandle mscorlibAssemblyRef, MemberReferenceHandle objectCtorMemberRef,
        MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        TypeReferenceHandle systemConsoleTypeRefHandle = metadata.AddTypeReference(
            mscorlibAssemblyRef,
            metadata.GetOrAddString("System"),
            metadata.GetOrAddString("Console"));
        
        // Get reference to Console.WriteLine(string) method.
        var consoleWriteLineSignature = new BlobBuilder();

        new BlobEncoder(consoleWriteLineSignature).MethodSignature().Parameters(1,
            returnType => returnType.Void(),
            parameters => parameters.AddParameter().Type().String());

        MemberReferenceHandle consoleWriteLineMemberRef = metadata.AddMemberReference(
            systemConsoleTypeRefHandle,
            metadata.GetOrAddString("WriteLine"),
            metadata.GetOrAddBlob(consoleWriteLineSignature));
            
        // Create signature for "void Main()" method.
        var mainSignature = new BlobBuilder();

        new BlobEncoder(mainSignature).
            MethodSignature().
            Parameters(0, returnType => returnType.Void(), parameters => { });
        
        var codeBuilder = new BlobBuilder();
        
        // Emit IL for Program::Main
        var flowBuilder = new ControlFlowBuilder();
        var il = new InstructionEncoder(codeBuilder, flowBuilder);

        // ldstr "hello"
        // il.LoadString(metadata.GetOrAddUserString("Hello, world"));
        for(var i = 2; i < 7; i++)
        {
            il.Call(MetadataTokens.MethodDefinitionHandle(i));
            il.Call(consoleWriteLineMemberRef);

        }

        il.LoadConstantI8(5);
        il.Call(MetadataTokens.MethodDefinitionHandle(7));

        il.Call(consoleWriteLineMemberRef);
        // call void [mscorlib]System.Console::WriteLine(string)
            
        
        // ret
        il.OpCode(ILOpCode.Ret);

        int mainBodyOffset = methodBodyStreamEncoder.AddMethodBody(il);
        codeBuilder.Clear();

        
        
        // Create method definition for Program::Main
        MethodDefinitionHandle mainMethodDef = metadata.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL,
            metadata.GetOrAddString("Main"),
            metadata.GetOrAddBlob(mainSignature),
            mainBodyOffset,
            parameterList: default(ParameterHandle));
        
        return mainMethodDef;
    }
    
    public static void GenerateCode(this Node node, CodeGeneratorContext codeGenerationContext)
    {
        if (node.Type == NodeType.Program)
            throw new InvalidOperationException();

        GetCodeGenerator(node).GenerateCode(node, codeGenerationContext);
    }
}