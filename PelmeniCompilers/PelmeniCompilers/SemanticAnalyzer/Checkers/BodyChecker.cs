﻿using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class BodyChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Body;
    
    public override void Check(Node node)
    {
        throw new NotImplementedException();
    }
}