using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using PelmeniCompilers.CodeGeneration.Generators;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration;

public class CodeGenerator
{
    private readonly Node _mainNode;

    public static readonly BlobContentId SContentId;
    public static readonly Guid SGuid;

    public CodeGenerator(Node mainNode)
    {
        if (mainNode.Type != NodeType.Program)
            throw new InvalidOperationException();
        _mainNode = mainNode;
    }

    static CodeGenerator()
    {
        SGuid = Guid.NewGuid();
        SContentId = new BlobContentId(SGuid, (uint)new Random().Next());
    }

    public void GenerateCode(string outputFile = "output.exe", bool dryRun = false)
    {
        // TODO
        /*if (string.IsNullOrWhiteSpace(entryMethod))
            throw new InvalidOperationException();*/

       

        var (metadataBuilder, ilBuilder, entryPoint) = _mainNode.GenerateProgram();

        if (!dryRun)
        {
            using var peStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            WritePEImage(peStream, metadataBuilder, ilBuilder, entryPoint);
        }
    }


    private static void WritePEImage(Stream peStream, MetadataBuilder metadataBuilder, BlobBuilder ilBuilder,
        MethodDefinitionHandle entryPointHandle)
    {
        // Create executable with the managed metadata from the specified MetadataBuilder.
        var peHeaderBuilder = new PEHeaderBuilder(
            imageCharacteristics: Characteristics.ExecutableImage
        );

        var peBuilder = new ManagedPEBuilder(
            peHeaderBuilder,
            new MetadataRootBuilder(metadataBuilder),
            ilBuilder,
            entryPoint: entryPointHandle,
            flags: CorFlags.ILOnly,
            deterministicIdProvider: content => SContentId);

        // Write executable into the specified stream.
        var peBlob = new BlobBuilder();
        BlobContentId contentId = peBuilder.Serialize(peBlob);
        peBlob.WriteContentTo(peStream);
    }

    private static MethodDefinitionHandle EmitHelloWorld(MetadataBuilder metadata, BlobBuilder ilBuilder)
    {
        // Create module and assembly for a console application.
        metadata.AddModule(
            0,
            metadata.GetOrAddString("ConsoleApplication.exe"),
            metadata.GetOrAddGuid(SGuid),
            default(GuidHandle),
            default(GuidHandle));

        metadata.AddAssembly(
            metadata.GetOrAddString("ConsoleApplication"),
            version: new Version(1, 0, 0, 0),
            culture: default(StringHandle),
            publicKey: default(BlobHandle),
            flags: 0,
            hashAlgorithm: AssemblyHashAlgorithm.None);

        // Create references to System.Object and System.Console types.
        AssemblyReferenceHandle mscorlibAssemblyRef = metadata.AddAssemblyReference(
            name: metadata.GetOrAddString("mscorlib"),
            version: new Version(4, 0, 0, 0),
            culture: default(StringHandle),
            publicKeyOrToken: metadata.GetOrAddBlob(
                new byte[] { 0xB7, 0x7A, 0x5C, 0x56, 0x19, 0x34, 0xE0, 0x89 }
            ),
            flags: default(AssemblyFlags),
            hashValue: default(BlobHandle));

        TypeReferenceHandle systemObjectTypeRef = metadata.AddTypeReference(
            mscorlibAssemblyRef,
            metadata.GetOrAddString("System"),
            metadata.GetOrAddString("Object"));

        TypeReferenceHandle systemConsoleTypeRefHandle = metadata.AddTypeReference(
            mscorlibAssemblyRef,
            metadata.GetOrAddString("System"),
            metadata.GetOrAddString("Console"));

        TypeReferenceHandle systemInt32TypeRefHandle = metadata.AddTypeReference(
            mscorlibAssemblyRef,
            metadata.GetOrAddString("System"),
            metadata.GetOrAddString("Int32"));

        #region Console.WriteLine

        // Get reference to Console.WriteLine(string) method.
        var consoleWriteLineSignature = new BlobBuilder();

        new BlobEncoder(consoleWriteLineSignature).MethodSignature().Parameters(1,
            returnType => returnType.Void(),
            parameters => parameters.AddParameter().Type().String());

        MemberReferenceHandle consoleWriteLineMemberRef = metadata.AddMemberReference(
            systemConsoleTypeRefHandle,
            metadata.GetOrAddString("WriteLine"),
            metadata.GetOrAddBlob(consoleWriteLineSignature));

        #endregion

        #region Console.Beep

        var consoleBeepSignature = new BlobBuilder();
        new BlobEncoder(consoleBeepSignature)
            .MethodSignature()
            .Parameters(0,
                returnType => returnType.Void(),
                parameters => { });
        var consoleBeepMemberRef = metadata.AddMemberReference(
            systemConsoleTypeRefHandle,
            metadata.GetOrAddString("Beep"),
            metadata.GetOrAddBlob(consoleBeepSignature));

        #endregion

        #region Console.ReadLine

        var consoleReadLineSignature = new BlobBuilder();

        new BlobEncoder(consoleReadLineSignature).MethodSignature().Parameters(0,
            returnType => returnType.Type().String(),
            parameters => { });

        MemberReferenceHandle consoleReadLineMemberRef = metadata.AddMemberReference(
            systemConsoleTypeRefHandle,
            metadata.GetOrAddString("ReadLine"),
            metadata.GetOrAddBlob(consoleReadLineSignature));

        #endregion

        #region Int32.ToString()

        var intToStringSignature = new BlobBuilder();
        new BlobEncoder(intToStringSignature).MethodSignature(isInstanceMethod: true)
            .Parameters(0, returnType => returnType.Type().String(), parameters => { });

        var intToStringMemberRef = metadata.AddMemberReference(
            systemInt32TypeRefHandle,
            metadata.GetOrAddString("ToString"),
            metadata.GetOrAddBlob(intToStringSignature));

        #endregion

        #region Int.Parse

        var intParseSignature = new BlobBuilder();
        new BlobEncoder(intParseSignature).MethodSignature().Parameters(1,
            returnType => returnType.Type().Int32(),
            parameters => parameters.AddParameter().Type().String());

        var intParseMemberRef = metadata.AddMemberReference(
            systemInt32TypeRefHandle,
            metadata.GetOrAddString("Parse"),
            metadata.GetOrAddBlob(intParseSignature));

        #endregion

        #region Object.ctor

        // Get reference to Object's constructor.
        var parameterlessCtorSignature = new BlobBuilder();

        new BlobEncoder(parameterlessCtorSignature).MethodSignature(isInstanceMethod: true)
            .Parameters(0, returnType => returnType.Void(), parameters => { });

        BlobHandle parameterlessCtorBlobIndex = metadata.GetOrAddBlob(parameterlessCtorSignature);

        MemberReferenceHandle objectCtorMemberRef = metadata.AddMemberReference(
            systemObjectTypeRef,
            metadata.GetOrAddString(".ctor"),
            parameterlessCtorBlobIndex);

        #endregion

        #region Program constructor

        var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);

        var codeBuilder = new BlobBuilder();
        var il = new InstructionEncoder(codeBuilder);

        // ldarg.0
        il.LoadArgument(0);

        // call instance void [mscorlib]System.Object::.ctor()
        il.Call(objectCtorMemberRef);

        // ret
        il.OpCode(ILOpCode.Ret);

        int ctorBodyOffset = methodBodyStream.AddMethodBody(il);
        codeBuilder.Clear();

        #endregion

        #region Main Signature

        // Create signature for "void Main()" method.
        var mainSignature = new BlobBuilder();

        BlobEncoder blobEncoder = new BlobEncoder(mainSignature);
        blobEncoder.MethodSignature().Parameters(0, returnType => returnType.Void(), parameters => { });

        #endregion

        #region Main Body

        var varBuilder = new BlobBuilder();
        var varEncoder = new BlobEncoder(varBuilder).LocalVariableSignature(2);
        varEncoder.AddVariable().Type().Int32();
        varEncoder.AddVariable().Type().Int32();

        // var col = new LocalVariableHandleCollection();
        // col.Append(varA).Append(varB);
        var ss = metadata.GetOrAddBlob(varBuilder);
        var sig = metadata.AddStandaloneSignature(ss);

        // Emit IL for Program::Main
        var flowBuilder = new ControlFlowBuilder();
        il = new InstructionEncoder(codeBuilder, flowBuilder);

        il.LoadConstantI8(0);
        il.StoreLocal(0);
        il.LoadConstantI8(1);
        il.StoreLocal(1);

        // ------------------------- IF a < b then write(A) else write(B)

        // labels
        LabelHandle elseLabel = il.DefineLabel();
        LabelHandle endLabel = il.DefineLabel();

        // load a
        il.Call(consoleReadLineMemberRef);
        il.Call(intParseMemberRef);
        il.StoreLocal(0);

        // load a
        il.LoadLocal(0);
        // load b
        il.LoadConstantI8(0);

        // if a < b => goto elseLabel ("if not" basically)
        il.Branch(ILOpCode.Blt, elseLabel);

        // then
        il.LoadString(metadata.GetOrAddUserString(">=0"));
        il.Call(consoleWriteLineMemberRef);

        il.Branch(ILOpCode.Br, endLabel);

        // else
        il.MarkLabel(elseLabel);
        il.LoadString(metadata.GetOrAddUserString("<0"));
        il.Call(consoleWriteLineMemberRef);

        il.MarkLabel(endLabel);

        // ---------- ENDIF

        // ------------------ WHILE a < b do write(a) a++

        var whileEndLabel = il.DefineLabel();
        var whileContLabel = il.DefineLabel();

        il.LoadConstantI8(0);
        il.StoreLocal(1);

        il.MarkLabel(whileContLabel);

        il.LoadLocal(0);
        il.LoadLocal(1);
        il.Branch(ILOpCode.Ble, whileEndLabel);

        // body

        //write(a)
        il.LoadLocalAddress(1);
        il.Call(intToStringMemberRef);
        il.Call(consoleWriteLineMemberRef);

        // a++
        il.LoadLocal(1);
        il.LoadConstantI8(1);
        il.OpCode(ILOpCode.Add);
        il.StoreLocal(1);

        il.Call(consoleBeepMemberRef);

        il.Branch(ILOpCode.Br, whileContLabel);

        // stop
        il.MarkLabel(whileEndLabel);

        // -------- ENDWHILE


        // ret
        il.OpCode(ILOpCode.Ret);
        int mainBodyOffset = methodBodyStream.AddMethodBody(il, 8, sig, MethodBodyAttributes.InitLocals);
        // codeBuilder.Clear();

        #endregion


        // Create method definition for Program::Main
        MethodDefinitionHandle mainMethodDef = metadata.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL,
            metadata.GetOrAddString("Main"),
            metadata.GetOrAddBlob(mainSignature),
            mainBodyOffset,
            parameterList: default(ParameterHandle));

        // var mainLocalScope = metadata.AddLocalScope(
        //     mainMethodDef, 
        //     MetadataTokens.ImportScopeHandle(1), 
        //     varB, 
        //     MetadataTokens.LocalConstantHandle(1), mainBodyOffset + varBuilder.Count, codeBuilder.Count);

        codeBuilder.Clear();

        // Create method definition for Program::.ctor
        metadata.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName,
            MethodImplAttributes.IL,
            metadata.GetOrAddString(".ctor"),
            parameterlessCtorBlobIndex,
            ctorBodyOffset,
            parameterList: default(ParameterHandle));

        // Create type definition for the special <Module> type that holds global functions
        metadata.AddTypeDefinition(
            default(TypeAttributes),
            default(StringHandle),
            metadata.GetOrAddString("<Module>"),
            baseType: default(EntityHandle),
            fieldList: MetadataTokens.FieldDefinitionHandle(1),
            methodList: mainMethodDef);

        // Create type definition for ConsoleApplication.Program
        metadata.AddTypeDefinition(
            TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit,
            metadata.GetOrAddString("ConsoleApplication"),
            metadata.GetOrAddString("Program"),
            baseType: systemObjectTypeRef,
            fieldList: MetadataTokens.FieldDefinitionHandle(1),
            methodList: mainMethodDef);

        return mainMethodDef;
    }
}