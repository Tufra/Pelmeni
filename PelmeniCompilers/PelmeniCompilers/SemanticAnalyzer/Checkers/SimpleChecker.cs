﻿using PelmeniCompilers.Models;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class SimpleChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Simple;
    public override void Check(Node node)
    {
        if (node.Children.Count == 1)
        {
            var subexpression = node.Children[0];
            subexpression.CheckSemantic();

            var computedSub = subexpression.BuildComputedExpression();

            var type = computedSub.ValueType;
            var value = computedSub.Value;
            var computed = new ComputedExpression(subexpression.Type, null, type, value)
            {
                Children = computedSub.Children
            };
            node.Children = new List<Node> { computed };
        }
        else
        {
            var leftOperand = node.Children[1];
            var rightOperand = node.Children[2];
            var oper = node.Children[0].Token!;

            leftOperand.CheckSemantic();
            rightOperand.CheckSemantic();
            
            ComputedExpression leftComputed = leftOperand.BuildComputedExpression();
            node.Children[1] = leftComputed;

            ComputedExpression rightComputed = rightOperand.BuildComputedExpression();
            node.Children[2] = rightComputed;

            var leftType = leftComputed.ValueType;
            var rightType = rightComputed.ValueType;
            var operType = Scanner.Scanner.TokenValueToGppgToken(oper);

            if (!((leftType == "integer" || leftType == "real") && (rightType == "integer" || rightType == "real")))
            {
                throw new InvalidOperationException(
                    $"The {operType} opertaion is defined only for integers and reals, {leftType} and {rightType} encountered at {oper.Location}"
                    );
            }

            switch (Scanner.Scanner.TokenValueToGppgToken(oper))
            {
                case Parser.Tokens.DIVIDE:
                    {
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {
                            
                            if (double.Parse(rightComputed.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture) == 0.0)
                            {
                                throw new InvalidOperationException($"Division by zero at {oper.Location}");
                            }

                            if (leftComputed.ValueType == "integer" && rightComputed.ValueType == "integer")
                            {
                                var val = 
                                    int.Parse(leftComputed.Value) / 
                                    int.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "integer", val.ToString())
                                {
                                    Children = node.Children
                                };

                                node.Children = new List<Node> { computed };
                            }
                            else
                            {
                                var val = 
                                    double.Parse(leftComputed.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture) / 
                                    double.Parse(rightComputed.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                                var computed = new ComputedExpression(NodeType.Summand, null, "real", val.ToString(System.Globalization.CultureInfo.InvariantCulture))
                                {
                                    Children = node.Children
                                };

                                node.Children = new List<Node> { computed };
                            }

                            
                        }
                        break;
                    }
                case Parser.Tokens.MOD:
                    {
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {
                            if (leftType == "real" || rightType == "real")
                            {
                                if (double.Parse(rightComputed.Value, System.Globalization.CultureInfo.InvariantCulture) == 0.0)
                                {
                                    throw new InvalidOperationException($"Division by zero at {oper.Location}");
                                }
                                var val = 
                                    double.Parse(leftComputed.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture) % 
                                    double.Parse(rightComputed.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                                var computed = new ComputedExpression(NodeType.Summand, null, "real", val.ToString())
                                {
                                    Children = node.Children
                                };

                                node.Children = new List<Node> { computed };
                            }
                            else
                            {
                                var val = int.Parse(leftComputed.Value) % int.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "integer", val.ToString())
                                {
                                    Children = node.Children
                                };

                                node.Children = new List<Node> { computed };
                            }
                            
                        }
                        break;
                    }
                case Parser.Tokens.MULTIPLY:
                    {
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {
                            if (leftType == "real" || rightType == "real")
                            {
                                var val = 
                                    double.Parse(leftComputed.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture) * 
                                    double.Parse(rightComputed.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                                var computed = new ComputedExpression(NodeType.Summand, null, "real", val.ToString())
                                {
                                    Children = node.Children
                                };

                                node.Children = new List<Node> { computed };
                            }
                            else
                            {
                                var val = int.Parse(leftComputed.Value) * int.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "integer", val.ToString())
                                {
                                    Children = node.Children
                                };

                                node.Children = new List<Node> { computed };
                            }
                            
                        }
                        break;
                    } 
                default:
                    throw new InvalidOperationException($"Undefined operation {operType} at {oper.Location}");
            }
        }
    }

    public override ComputedExpression BuildComputedExpression(Node node)
    {
        if (node.Children.Count == 1) // Expression
        {
            var child = (ComputedExpression)node.Children[0];
            var computed = new ComputedExpression(node.Type, child.Token, child.ValueType, child.Value)
            {
                Children = node.Children
            };
            return computed;
        }

        // Summand OPERATOR Simple
        var left = (ComputedExpression)node.Children[1];
        var right = (ComputedExpression)node.Children[2];

        if (left.ValueType == "real" || right.ValueType == "real")
        {
            var computed = new ComputedExpression(node.Type, null, "real", null)
            {
                Children = node.Children
            };
            return computed;        
        }
        else
        {
            var computed = new ComputedExpression(node.Type, null, "integer", null)
            {
                Children = node.Children
            };
            return computed;
        }
    }
}