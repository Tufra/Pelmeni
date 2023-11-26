using System.Collections.Immutable;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using PelmeniCompilers.CodeGeneration;
using PelmeniCompilers.CodeGeneration.Generators;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
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

        codeGenerationContext.ObjectTypeHandle = systemObjectTypeRef;

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


        metadataBuilder.AddTypeDefinition(
            default,
            default,
            metadataBuilder.GetOrAddString("<Module>"),
            baseType: default,
            fieldList: MetadataTokens.FieldDefinitionHandle(1),
            methodList: MetadataTokens.MethodDefinitionHandle(1));

        foreach (var child in node.Children.Where(node => node.Type == NodeType.TypeDeclaration))
        {
            child.GenerateCode(codeGenerationContext);
        }

        var mainMethodDef = GenerateEntryPoint(metadataBuilder, ilBuilder, mscorlibAssemblyRef, objectCtorMemberRef,
            methodBodyStream);

        foreach (var child in node.Children.Where(node => node.Type != NodeType.TypeDeclaration))
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


        // Create type definition for ConsoleApplication.Program
        metadataBuilder.AddTypeDefinition(
            TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit,
            metadataBuilder.GetOrAddString("ConsoleApplication"),
            metadataBuilder.GetOrAddString("Program"),
            baseType: systemObjectTypeRef,
            fieldList: MetadataTokens.FieldDefinitionHandle(codeGenerationContext.LastFieldIndex),
            methodList: mainMethodDef);


        return (metadataBuilder, ilBuilder, mainMethodDef);
    }

    private static MethodDefinitionHandle GenerateEntryPoint(MetadataBuilder metadata, BlobBuilder ilBuilder,
        AssemblyReferenceHandle mscorlibAssemblyRef, MemberReferenceHandle objectCtorMemberRef,
        MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        //
        // STDLIB REF
        //
        
        GenerateStdLibReferences("PelmeniLib", "PelmeniLib", metadata);
        
        //
        // END STDLIB REF
        //
        
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
        // for(var i = 2; i < 7; i++)
        // {
        //     il.Call(MetadataTokens.MethodDefinitionHandle(i));
        //     il.Call(consoleWriteLineMemberRef);

        // }

        il.LoadConstantI8(-5);
        il.Call(MetadataTokens.MethodDefinitionHandle(4));
        // il.Call(BaseNodeCodeGenerator.GeneratedRoutines["IntToString"]);
        
        il.Call(BaseNodeCodeGenerator.GeneratedRoutines["Print"]);
        
        // il.Call(intToStringMemberRef);
        // il.Call(printMemberRef);
        //
        // il.Call(readLineMemberRef);
        // il.Call(printMemberRef);

        // il.Call(consoleWriteLineMemberRef);
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

    private static void GenerateStdLibReferences(string name, string namespace_, MetadataBuilder metadata)
    {
        
        var libAssemblyRef = metadata.AddAssemblyReference(
            name: metadata.GetOrAddString("PelmeniLib"),
            version: new Version(1, 0, 0, 0),
            culture: default(StringHandle),
            publicKeyOrToken: default,
            flags: default(AssemblyFlags),
            hashValue: default(BlobHandle));
        
        var assembly = Assembly.Load(name);
        var userTypes = assembly.GetTypes()
            .Where(t => t.Namespace == namespace_)
            .Where(t => !t.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), true));
        foreach (var type in userTypes)
        {

            var identifier = type.Name;

            TypeReferenceHandle typeRefHandle = metadata.AddTypeReference(
                libAssemblyRef,
                metadata.GetOrAddString(namespace_),
                metadata.GetOrAddString(identifier));

            BaseNodeCodeGenerator.GeneratedRecords.Add(identifier, typeRefHandle);

            var methods = type.GetMethods();

            if (methods.Length > 0)
            {
                foreach (var method in methods)
                {
                    if (!method.IsStatic)
                    {
                        continue;
                    }

                    var methodName = method.Name;

                    var methodSignature = new BlobBuilder();

                    new BlobEncoder(methodSignature).MethodSignature().Parameters(1,
                        returnType => EncodeReturnType(returnType, method.ReturnParameter),
                        parameters => EncodeParameters(parameters, method.GetParameters()));

                    MemberReferenceHandle methodMemberRef = metadata.AddMemberReference(
                        typeRefHandle,
                        metadata.GetOrAddString(methodName),
                        metadata.GetOrAddBlob(methodSignature));

                    BaseNodeCodeGenerator.GeneratedRoutines.Add(methodName, methodMemberRef);
                }
            }
        }
    
    }

    private static void EncodeReturnType(ReturnTypeEncoder returnTypeEncoder, ParameterInfo returnType)
    {
        var typeConversion = new Dictionary<string, string>
        {
            { "System.Int32", "integer" },
            { "Int32", "integer" },
            { "System.Int64", "integer" },
            { "Int64", "integer" },
            { "System.Boolean", "boolean" },
            { "Boolean", "boolean" },
            { "System.Double", "real" },
            { "Double", "real" },
            { "System.Float", "real" },
            { "Float", "real" },
            { "System.Char", "char" },
            { "Char", "char" },
            { "System.String", "string" },
            { "String", "string" },
            { "Void", "None" }
        };

        var typeStr = returnType.ParameterType.Name;
        var success = typeConversion.TryGetValue(typeStr, out var trueType);
        if (success)
        {
            switch (trueType)
            {
                case "integer":
                {
                    returnTypeEncoder.Type().Int64();
                    break;
                }
                case "real":
                {
                    returnTypeEncoder.Type().Double();
                    break;
                }
                case "boolean":
                {
                    returnTypeEncoder.Type().Boolean();
                    break;
                }
                case "char":
                {
                    returnTypeEncoder.Type().Char();
                    break;
                }
                case "string":
                {
                    returnTypeEncoder.Type().String();
                    break;
                }
                case "None":
                {
                    returnTypeEncoder.Void();
                    break;
                }
            }
        }
        else
        {
            if (returnType.ParameterType.IsArray)
            {
                var elementType = returnType.ParameterType.GetElementType().Name;
                
                Action<SignatureTypeEncoder> elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.VoidPointer(); };
                Action<ArrayShapeEncoder> arrayShapeDelegate = delegate (ArrayShapeEncoder shapeEncoder) {
                    var bounds = new List<int> { 0 };
                    shapeEncoder.Shape(
                        1, 
                        new ImmutableArray<int>(), 
                        new ImmutableArray<int>() { 0 }); };

                

                switch (elementType)
                {
                    case "integer":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Int64(); };
                        break;
                    }
                    case "real":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Double(); };
                        break;
                    }
                    case "boolean":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Boolean(); };
                        break;
                    }
                    case "char":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Char(); };
                        break;
                    }
                    case "string":
                    {
                        elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.String(); };
                        break;
                    }
                    default:
                    {
                        EntityHandle record;
                        success = BaseNodeCodeGenerator.GeneratedRecords.TryGetValue(elementType, out record);
                        if (success)
                        {
                            elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Type(record, false); };
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unknown type {elementType}");
                        }
                        break;
                    }
                }

                returnTypeEncoder.Type().Array(elementTypeDelegate, arrayShapeDelegate);
            }
            else
            {
                var actualName = typeStr.Split('.')[1];
                var recordsSearchSuccess =
                    BaseNodeCodeGenerator.GeneratedRecords.TryGetValue(actualName, out var acturalRecord);
                if (recordsSearchSuccess)
                {
                    returnTypeEncoder.Type().Type(acturalRecord, false);
                }
                else
                {
                    throw new InvalidExpressionException($"unknown type {typeStr}");
                }
            }
                
            
        }
    }

    private static void EncodeParameters(ParametersEncoder parametersEncoder, ParameterInfo[] parameters)
    {
        var typeConversion = new Dictionary<string, string>
        {
            { "System.Int32", "integer" },
            { "Int32", "integer" },
            { "System.Int64", "integer" },
            { "Int64", "integer" },
            { "System.Boolean", "boolean" },
            { "Boolean", "boolean" },
            { "System.Double", "real" },
            { "Double", "real" },
            { "System.Float", "real" },
            { "Float", "real" },
            { "System.Char", "char" },
            { "Char", "char" },
            { "System.String", "string" },
            { "String", "string" },
            { "Void", "None" }
        };

        foreach (var parameter in parameters)
        {
            var typeStr = parameter.ParameterType.Name;
            var success = typeConversion.TryGetValue(typeStr, out var trueType);
            if (success)
            {
                switch (trueType)
                {
                    case "integer":
                    {
                        parametersEncoder.AddParameter().Type().Int64();
                        break;
                    }
                    case "real":
                    {
                        parametersEncoder.AddParameter().Type().Double();
                        break;
                    }
                    case "boolean":
                    {
                        parametersEncoder.AddParameter().Type().Boolean();
                        break;
                    }
                    case "char":
                    {
                        parametersEncoder.AddParameter().Type().Char();
                        break;
                    }
                    case "string":
                    {
                        parametersEncoder.AddParameter().Type().String();
                        break;
                    }
                }
            }
            else
            {
                if (parameter.ParameterType.IsArray)
                {
                    var elementType = parameter.ParameterType.GetElementType().Name;
                    
                    Action<SignatureTypeEncoder> elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.VoidPointer(); };
                    Action<ArrayShapeEncoder> arrayShapeDelegate = delegate (ArrayShapeEncoder shapeEncoder) {
                        var bounds = new List<int> { 0 };
                        shapeEncoder.Shape(
                            1, 
                            new ImmutableArray<int>(), 
                            new ImmutableArray<int>() { 0 }); };

                    

                    switch (elementType)
                    {
                        case "integer":
                        {
                            elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Int64(); };
                            break;
                        }
                        case "real":
                        {
                            elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Double(); };
                            break;
                        }
                        case "boolean":
                        {
                            elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Boolean(); };
                            break;
                        }
                        case "char":
                        {
                            elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Char(); };
                            break;
                        }
                        case "string":
                        {
                            elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.String(); };
                            break;
                        }
                        default:
                        {
                            EntityHandle record;
                            success = BaseNodeCodeGenerator.GeneratedRecords.TryGetValue(elementType, out record);
                            if (success)
                            {
                                elementTypeDelegate = delegate (SignatureTypeEncoder typeEncoder) { typeEncoder.Type(record, false); };
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unknown type {elementType}");
                            }
                            break;
                        }
                    }

                    parametersEncoder.AddParameter().Type().Array(elementTypeDelegate, arrayShapeDelegate);
                }
                else
                {
                    var actualName = typeStr.Split('.')[1];
                    var recordsSearchSuccess =
                        BaseNodeCodeGenerator.GeneratedRecords.TryGetValue(actualName, out var acturalRecord);
                    if (recordsSearchSuccess)
                    {
                        parametersEncoder.AddParameter().Type().Type(acturalRecord, false);
                    }
                    else
                    {
                        throw new InvalidExpressionException($"unknown type {typeStr}");
                    }
                }
                
            }
        }
    }
}