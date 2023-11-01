using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class ModuleChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Module;
    public override void Check(Node node)
    {
        //Scope.AddFrame(new Module(node.Children![0].Token!.Value));
        //CheckChildren(node);
        return;
    }
}