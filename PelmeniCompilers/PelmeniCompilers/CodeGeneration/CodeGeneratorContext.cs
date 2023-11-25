using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

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
    public bool IsAddress { get; set; } = false;
    public TypeReferenceHandle ObjectTypeHandle { get; set; }
    public int LastFieldIndex { get; set; } = 1;
}