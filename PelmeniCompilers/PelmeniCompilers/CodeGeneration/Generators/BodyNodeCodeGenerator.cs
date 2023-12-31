using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class BodyNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.Body;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var children = node.Children;

        Chain.Push(node);
        
        foreach (var child in children)
        {
            if(child.Type == NodeType.VariableDeclaration)
                continue;
            child.GenerateCode(codeGeneratorContext);
        }

        Chain.Pop();
    }

}