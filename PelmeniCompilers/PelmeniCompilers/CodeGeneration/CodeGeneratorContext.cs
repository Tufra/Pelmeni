using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace PelmeniCompilers.CodeGeneration;

public class CodeGeneratorContext
{
    public MetadataBuilder MetadataBuilder { get; set; } = null!;
    public BlobBuilder IlBuilder { get; set; } = null!;
    
    public MethodBodyStreamEncoder MethodBodyStreamEncoder { get; set; }
}