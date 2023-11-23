using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace PelmeniCompilers.CodeGeneration;

public class CodeGeneratorContext
{
    public MetadataBuilder MetadataBuilder { get; set; } = null!;
    public BlobBuilder IlBuilder { get; set; } = null!;
    public InstructionEncoder InstructionEncoder { get; set; }
    public LocalVariablesEncoder VarEncoder { get; set; }
    public MethodBodyStreamEncoder MethodBodyStreamEncoder { get; set; }
}