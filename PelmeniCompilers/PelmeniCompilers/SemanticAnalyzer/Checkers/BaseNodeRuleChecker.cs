using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public abstract class BaseNodeRuleChecker
{
    public abstract NodeType CheckingNodeType { get; }

    protected static Scope Scope { get; set; }
    protected static Stack<Node> Chain { get; set; }

    static BaseNodeRuleChecker()
    {
        Scope = new Scope();
        Chain = new Stack<Node>();
    }

    public abstract void Check(Node node);

    protected static void CheckChildren(Node node)
    {
        if (node.Children is null) return;

        foreach (var child in node.Children)
        {
            child.CheckSemantic();
        }
    }
}