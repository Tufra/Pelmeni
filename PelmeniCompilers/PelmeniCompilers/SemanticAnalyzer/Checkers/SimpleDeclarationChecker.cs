﻿using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class SimpleDeclarationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.SimpleDeclaration;
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}