using System.Reflection.Metadata;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.ExtensionsMethods;

namespace PelmeniCompilers.CodeGeneration.Generators;

public class ModifiablePrimaryNodeCodeGenerator : BaseNodeCodeGenerator
{
    public override NodeType GeneratingCodeNodeType => NodeType.ModifiablePrimary;

    public override void GenerateCode(Node node, CodeGeneratorContext codeGeneratorContext)
    {
        var il = codeGeneratorContext.InstructionEncoder;
        var metadata = codeGeneratorContext.MetadataBuilder;

        var children = node.Children;
        var baseIdentifier = children[0].Token!.Value;
        var baseVarIndex = -1;
        if (codeGeneratorContext.LocalVariablesIndex!.ContainsKey(baseIdentifier))
        {
            codeGeneratorContext.LocalVariablesIndex.TryGetValue(baseIdentifier, out baseVarIndex);
            il.LoadLocal(baseVarIndex);
        }
        else if (codeGeneratorContext.ArgumentsIndex!.ContainsKey(baseIdentifier))
        {
            codeGeneratorContext.ArgumentsIndex.TryGetValue(baseIdentifier, out baseVarIndex);
            il.LoadArgument(baseVarIndex);
        }

        foreach (var child in children.Skip(1))
        {
            if (child.Type == NodeType.MemberAccess)
            {
                throw new NotImplementedException();
            }
            else if (child.Type == NodeType.ArrayAccess)
            {
                throw new NotImplementedException();
            }
        }
    }

    
}