using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class BodyNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Body;
    public static MethodDefinitionHandle hanldle;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var children = node.Children;

        foreach (var child in children)
        {
            child.GenerateCode(codeGeneratorContext);
        }
    }

}