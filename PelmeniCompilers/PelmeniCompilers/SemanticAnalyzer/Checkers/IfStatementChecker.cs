﻿using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class IfStatementChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.IfStatement;

    public override void Check(Node node)
    {
        var condition = node.Children[0]!;
        var body = node.Children[1]!;
        var elseBody = node.Children[2]!;

        condition.CheckSemantic();
        body.CheckSemantic();
        elseBody.CheckSemantic();

        var computedCondition = condition.BuildComputedExpression();

        if (computedCondition.ValueType != "boolean")
        {
            throw new InvalidOperationException(
                $"Only booleans allowed as condition");
        }

        node.Children[0] = computedCondition;
    }
}