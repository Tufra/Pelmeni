using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public abstract class BaseNodeCodeGenerator
{
    public static readonly Dictionary<string, EntityHandle> GeneratedRoutines = new();
    public static readonly Dictionary<string, EntityHandle> GeneratedRecords = new();
    
    public abstract NodeType GeneratingCodeNodeType { get; }

    public abstract void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext);

}