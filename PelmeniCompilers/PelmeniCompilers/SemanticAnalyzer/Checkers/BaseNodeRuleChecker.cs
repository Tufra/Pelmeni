using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public abstract class BaseNodeRuleChecker
{
    protected static Stack<HashSet<Unit>> StackFrame { get; set; } = null!;
    public abstract NodeType CheckingNodeType { get; }

    public abstract void Check(Node node, Stack<HashSet<Unit>> frame);

    /// <summary>
    /// Проверка на наличие в фрайме
    /// </summary>
    /// <param name="frames"></param>
    /// <returns></returns>
    protected bool CheckScopeFrame()
    {
        throw new NotImplementedException();
    }

    public bool CanCheck(NodeType type) => type == CheckingNodeType;
}