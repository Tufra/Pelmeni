using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class TypeTailChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.TypeTail;

    public override void Check(Node node)
    {
        if (node.Children.Count == 1)
        {
            
            var type = node.Children[0];
            if (type.Type == NodeType.Token)
            {
                var typeStr = type.Token!.Value;
                if (TypeDeclarationChecker.IsPrimitiveType(typeStr))
                {
                    return;
                }
                var record = GetRecordOrThrowIfNotDeclared(typeStr, node.Children[0].Token!.Location);
                
                NodeOptimizationExtension.RecordUsage[record.Node] = true;
            }
            else if (type.Type == NodeType.ArrayType)
            {
                type.CheckSemantic();
            }
            else
            {
                throw new InvalidOperationException(
                    $"Illegal type {type.Type}");
            }
        }
    }
}