using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;
using PelmeniCompilers.Scanner;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class SummandChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Summand;

    public override void Check(Node node)
    {
        if (node.Children[0].Type == NodeType.Sign) 
        {
            var sign = node.Children[0];
            var subexpression = node.Children[1];
            subexpression.CheckSemantic();

            if (subexpression.Type == NodeType.ModifiablePrimary)
            {
                var computedSub = subexpression.BuildComputedExpression();
                var computed = new ComputedExpression(NodeType.Summand, computedSub.Token, computedSub.ValueType, computedSub.Value)
                {
                    Children = computedSub.Children
                };
                node.Children[1] = computed;

                return;
            }
            else if (subexpression.Type == NodeType.RoutineCall)
            {
                var routine = GetRoutineOrThrowIfNotDeclared(subexpression);

                var computed = new ComputedExpression(subexpression.Type, null, routine.ReturnType, null)
                {
                    Children = subexpression.Children
                };
                node.Children[1] = computed;
                return;
            }

            var token = subexpression.Token!;
            switch (Scanner.Scanner.TokenValueToGppgToken(token))
            {
                case Parser.Tokens.INTEGER_LITERAL:
                    {
                        var val = int.Parse(token.Value);
                        if (!sign.IsTerminal()) 
                        {
                            token.Value = (-val).ToString();
                        }
                        var computed = new ComputedExpression(node.Type, token, "integer", token.Value);
                        node.Children = new List<Node> { computed };
                        break;
                    }
                case Parser.Tokens.REAL_LITERAL:
                    {
                        var val = double.Parse(token.Value);
                        if (!sign.IsTerminal()) 
                        {
                            token.Value = (-val).ToString();
                        }
                        var computed = new ComputedExpression(node.Type, token, "real", token.Value);
                        node.Children = new List<Node> { computed };
                        break;
                    }
                case Parser.Tokens.CHAR_LITERAL:
                    {
                        if (!sign.IsTerminal()) 
                        {
                            throw new InvalidOperationException($"Char cannot be negative at {token.Location}");
                        }
                        var computed = new ComputedExpression(node.Type, token, "char", token.Value);
                        node.Children = new List<Node> { computed };
                        break;
                    }
                case Parser.Tokens.STRING_LITERAL:
                    {
                        if (!sign.IsTerminal()) 
                        {
                            throw new InvalidOperationException($"String cannot be negative at {token.Location}");
                        }
                        var computed = new ComputedExpression(node.Type, token, "string", token.Value);
                        node.Children = new List<Node> { computed };
                        break;
                    }
                case Parser.Tokens.TRUE:
                    {
                        if (!sign.IsTerminal()) 
                        {
                            throw new InvalidOperationException($"Boolean cannot be negative at {token.Location}");
                        }
                        var computed = new ComputedExpression(node.Type, token, "boolean", token.Value);
                        node.Children = new List<Node> { computed };
                        break;
                    }
                case Parser.Tokens.FALSE:
                    {
                        if (!sign.IsTerminal()) 
                        {
                            throw new InvalidOperationException($"Boolean cannot be negative at {token.Location}");
                        }
                        var computed = new ComputedExpression(node.Type, token, "boolean", token.Value);
                        node.Children = new List<Node> { computed };
                        break;
                    }
                default:
                    throw new ArgumentException($"Illegal primary token {token}");
            }

        }
        else
        {
            var subexpression = node.Children[0];
            subexpression.CheckSemantic();
            var computed = node.BuildComputedExpression();
            
            node.Children = new List<Node> { computed };

        }
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        ComputedExpression child;
        if (node.Children.Count == 1) // Expression
        {
            child = (ComputedExpression)node.Children[0];
        }
        else // Sign Primary
        {
            child = (ComputedExpression)node.Children[1];
        }

        var computed = new ComputedExpression(node.Type, child.Token, child.ValueType, child.Value)
        {
            Children = node.Children
        };
        return computed;
        
    }
}