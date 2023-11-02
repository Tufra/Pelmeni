using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.ShiftReduceParser;

namespace PelmeniCompilers.SemanticAnalyzer;

public class Scope
{
    private readonly Stack<HashSet<VariableVirtualTableEntry>> _scope = new();

    public void AddFrame(params VariableVirtualTableEntry[] variables)
    {
        var frame = new HashSet<VariableVirtualTableEntry>();
        foreach (var scopeUnit in variables)
        {
            frame.Add(scopeUnit);
        }

        _scope.Push(frame);
    }


    public void AddToLastFrame(VariableVirtualTableEntry variable)
    {
        var lastFrame = _scope.Peek();
        lastFrame.Add(variable);
    }

    public void RemoveLastFrame()
    {
        _scope.Pop();
    }

    public bool Contains(string identifier, int depth = 0)
    {
        return Get(identifier, depth) is not null;
    }

    public VariableVirtualTableEntry? Get(string identifier, int depth = 0)
    {
        if (depth <= 0)
            depth = int.MaxValue;
        var currentDepth = 1;

        foreach (var frame in _scope.TakeWhile(frame => currentDepth <= depth))
        {
            var variable = frame.FirstOrDefault(e => e.Name == identifier);
            if (variable is not null)
                return variable;
            currentDepth++;
        }

        return null;
    }
}