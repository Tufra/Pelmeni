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
            version: new Version(4, 0, 0, 0),
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
        
        // Подключчить другие референсы


        foreach (var child in node.Children)     
        {
            child.GenerateCode(metadataBuilder, ilBuilder);
        }

        var mainMethodDef = GenerateEntryPoint();
        
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

    private static MethodDefinitionHandle GenerateEntryPoint()
    {
        return new MethodDefinitionHandle();
    }

    public static void GenerateCode(this Node node, MetadataBuilder metadataBuilder, BlobBuilder ilBuilder)
    {
        if (node.Type == NodeType.Program)
            throw new InvalidOperationException();
        
        GetCodeGenerator(node).GenerateCode(node, metadataBuilder, ilBuilder);
    }
}