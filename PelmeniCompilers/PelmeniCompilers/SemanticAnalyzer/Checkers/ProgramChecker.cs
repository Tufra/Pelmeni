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
        CheckChildren(node);
    }
}