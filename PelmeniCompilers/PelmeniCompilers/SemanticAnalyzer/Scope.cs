namespace PelmeniCompilers.SemanticAnalyzer;

public class Scope
{
    private readonly Stack<HashSet<ScopeUnit.ScopeUnit>> _scope = new();

    public void AddFrame(params ScopeUnit.ScopeUnit[] units)
    {
        var frame = new HashSet<ScopeUnit.ScopeUnit>();
        foreach (var scopeUnit in units)
        {
            frame.Add(scopeUnit);
        }
        _scope.Push(frame);
    }

    public void AddToLastFrame(params ScopeUnit.ScopeUnit[] units)
    {
        var lastFrame = _scope.Peek();
        foreach (var scopeUnit in units)
        {
            lastFrame.Add(scopeUnit);
        }
    }

    public void RemoveLastFrame()
    {
        _scope.Pop();
    }

    public bool Contains(ScopeUnit.ScopeUnit unit, int depth = 0)
    {
        if(depth <= 0)
            depth = int.MaxValue;
        var currentDepth = 1;
        
        foreach (var frame in _scope.TakeWhile(frame => currentDepth <= depth))
        {
            
            
            currentDepth++;
        }

        throw new NotImplementedException();
    }
        
}