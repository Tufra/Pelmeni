﻿using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class MemberAccessChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.MemberAccess;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}