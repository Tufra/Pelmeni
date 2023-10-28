﻿using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Rules;

public abstract class BaseSemanticRule
{
    public abstract NodeType CheckingNodeType { get; }

    public abstract void Check(Stack<HashSet<Unit>> frames);

    /// <summary>
    /// Проверка на наличие в фрайме
    /// </summary>
    /// <param name="frames"></param>
    /// <returns></returns>
    protected bool CheckScopeFrame(Stack<HashSet<Unit>> frames)
    {
        
    }
    public bool CanCheck(NodeType type) => type == CheckingNodeType;
}