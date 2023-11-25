using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public abstract class BaseNodeCodeGenerator
{
    public static readonly Dictionary<string, MethodDefinitionHandle> GeneratedRoutines = new();
    public static readonly Dictionary<string, TypeDefinitionHandle> GeneratedRecords = new();
    protected static Stack<Node> Chain { get; set; } = new();
    
    public abstract NodeType GeneratingCodeNodeType { get; }

    public abstract void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext);

}