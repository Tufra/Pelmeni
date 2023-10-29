using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ModuleChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Module;
    public override void Check(Node node, Stack<HashSet<Unit>> frame)
    {
        var lastScope = frame.Peek();
        lastScope.Add(new Module(node.Children![0].Token!.Value));
    }
}