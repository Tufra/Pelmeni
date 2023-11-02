using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ProgramChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Program;
    public override void Check(Node node)
    {
        Scope.AddFrame();
        Chain.Push(node);

        var module = node.Children[0];
        var imports = node.Children[1];
        
        imports.CheckSemantic();

        node.Children.Remove(module);
        node.Children.Remove(imports);
        
        foreach (var child in node.Children)
        {
            child.CheckSemantic();
        }
        
        Chain.Pop();
        Scope.RemoveLastFrame();
    }
}