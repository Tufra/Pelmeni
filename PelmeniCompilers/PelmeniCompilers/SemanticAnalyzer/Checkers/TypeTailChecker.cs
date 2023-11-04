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
            var type = node.Children[0].Token!.Value;

            // if primitive type
            if (type == "integer" || type == "real" || type == "boolean" || type == "char")
            {
                return;
            }

            // if array
            // TODO

            // if record
            GetRecordOrThrowIfNotDeclared(type, node.Children[0].Token!.Location);
        }
    }
}