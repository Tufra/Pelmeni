using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ImportsChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Imports;

    public override void Check(Node node)
    {
        if (node.IsTerminal()) return;

        foreach (var programChild in node.Children)
        {
            var importsNode = GetImportsNode(programChild);
            importsNode.CheckSemantic();
        }
        
        var program = Chain.Peek();
        program.Children.InsertRange(2, node.Children[0].Children.Skip(2)); //Imports -> Program, Skip (Module, Imports)
        node.Children = new List<Node>();
    }

    private static Node GetImportsNode(Node node) => node.Children![1];
}