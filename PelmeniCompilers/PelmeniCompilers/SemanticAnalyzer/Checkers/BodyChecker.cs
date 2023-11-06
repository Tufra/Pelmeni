using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class BodyChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Body;
    
    public override void Check(Node node)
    {
        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            child.CheckSemantic();
            switch (child.Type)
            {
                case NodeType.IfStatement:
                {
                    var cond = (ComputedExpression)child.Children[0]!;
                    if (cond.Value is not null)
                    {
                        if (cond.Value == "True")
                        {
                            children[i] = child.Children[1];
                        }
                        else if (cond.Value == "False")
                        {
                            var elseTail = child.Children[2];
                            if (elseTail.Children.Count == 1)
                            {
                                children[i] = elseTail.Children[1];
                            }
                        }
                    }
                    break;
                }
                case NodeType.ForLoop:
                {
                    var range = (ComputedExpression)child.Children[1]!;
                    if (range.Value is not null && range.Value == "0")
                    {
                        children[i] = new Node(NodeType.Body, new List<Node> { });
                    }
                    break;
                }
                case NodeType.WhileLoop:
                {
                    var cond = (ComputedExpression)child.Children[0]!;
                    if (cond.Value is not null)
                    {
                        if (cond.Value == "False")
                        {
                            children[i] = new Node(NodeType.Body, new List<Node> { });
                        }
                    }
                    break;
                }
            }
        }
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        var statements = node.Children;
        foreach (var child in statements)
        {
            if (child.Type == NodeType.Return)
            {
                var computedReturn = child.BuildComputedExpression();
                var computed = new ComputedExpression(node.Type, null, computedReturn.ValueType, computedReturn.Value)
                {
                    Children = node.Children
                };
                return computed;
            }

            // TODO if return in loop or condition
        }
        var _computed = new ComputedExpression(node.Type, null, "None", null)
        {
            Children = node.Children
        };
        return _computed;
    }
    
}