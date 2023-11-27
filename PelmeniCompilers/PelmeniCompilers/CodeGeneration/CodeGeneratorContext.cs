using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration;

public class CodeGeneratorContext
{
    public MetadataBuilder MetadataBuilder { get; set; } = null!;
    public BlobBuilder IlBuilder { get; set; } = null!;
    public MethodBodyStreamEncoder MethodBodyStreamEncoder { get; set; }
    public InstructionEncoder InstructionEncoder { get; set; }
    public LocalVariablesEncoder? VarEncoder { get; set; }
    public Dictionary<string, int>? LocalVariablesIndex { get; set; }
    public int LastVariableIndex { get; set; } = -1;
    public Dictionary<string, int>? ArgumentsIndex { get; set; }
    public int LastArgumentIndex { get; set; } = -1;
    public bool IsLeftValue { get; set; } = false;
    public bool IsValueObsolete { get; set; } = true;
    public TypeReferenceHandle ObjectTypeHandle { get; set; }
    public int LastFieldIndex { get; set; } = 1;
    public int LastRoutineIndex { get; set; } = 1;
    public VariableType VariableType { get; set; }
    public int TokenOffset { get; set; } = 0;
    public AssemblyReferenceHandle MscorlibReference { get; set; }
}