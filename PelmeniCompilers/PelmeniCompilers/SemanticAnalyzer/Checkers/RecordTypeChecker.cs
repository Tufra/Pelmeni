﻿using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RecordTypeChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.RecordType;

    public override void Check(Node node)
    {
        Scope.AddFrame();
        Chain.Push(node);

        foreach (var member in node.Children)
        {
            member.CheckSemantic();
        }

        Scope.RemoveLastFrame();
        Chain.Pop();
    }

    public static List<VariableVirtualTableEntry> GetMembers(Node node)
    {
        var members = new List<VariableVirtualTableEntry>();
        
        Scope.AddFrame();
        Chain.Push(node);

        foreach (var member in node.Children)
        {
            var entry = VariableDeclarationChecker.BuildVirtualTableEntry(member);
            members.Add(entry);
        }

        Scope.RemoveLastFrame();
        Chain.Pop();

        return members;
    }
}