using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.CodeGeneration.Generators;

public abstract class BaseNodeCodeGenerator
{
    public static readonly Dictionary<string, MemberReferenceHandle> GeneratedRoutines = new();
    public static readonly Dictionary<string, TypeReferenceHandle> GeneratedRecords = new();
    
    public abstract NodeType GeneratingCodeNodeType { get; }

    public abstract void GenerateCode(Node node, MetadataBuilder metadataBuilder, BlobBuilder ilBuilder);

}