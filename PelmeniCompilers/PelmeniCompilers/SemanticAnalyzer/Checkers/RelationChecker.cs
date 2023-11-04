using PelmeniCompilers.Models;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RelationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Relation;
    public override void Check(Node node)
    {
        if (node.Children.Count == 1) 
        {
            var subexpression = node.Children[0];
            subexpression.CheckSemantic();
            
            var computedSub = subexpression.BuildComputedExpression();

            if (computedSub.Value is null)
            {
                computedSub.Children = new List<Node> { subexpression };
            }
            node.Children = new List<Node> { computedSub };
        }
        else if (node.Children.Count == 2) // Not Relation
        {
            var oper = node.Children[1].Token!;
            var subexpression = node.Children[1];
            subexpression.CheckSemantic();

            var computedSub = subexpression.BuildComputedExpression();
            
            if (computedSub.ValueType == "boolean")
            {
                var val = computedSub.Value;

                var computed = new ComputedExpression(
                        subexpression.Type, 
                        null, 
                        computedSub.ValueType, 
                        null);

                if (val is not null)
                {
                    var boolVal = ! bool.Parse(val);
                    computed.Value = boolVal.ToString();
                }

                node.Children = new List<Node> { computed };
            }
            else
            {
                throw new InvalidOperationException(
                    $"The NOT opertaion is defined only for booleans, {computedSub.ValueType} encountered at {oper.Location}"
                    );
            }
        }
        else
        {
            var leftOperand = node.Children[1];
            var rightOperand = node.Children[2];
            var oper = node.Children[0].Token!;

            leftOperand.CheckSemantic();
            rightOperand.CheckSemantic();
            
            var leftComputed = leftOperand.BuildComputedExpression();
            node.Children[1] = leftComputed;

            var rightComputed = rightOperand.BuildComputedExpression();
            node.Children[2] = rightComputed;

            var leftType = leftComputed.ValueType;
            var rightType = rightComputed.ValueType;
            var operType = Scanner.Scanner.TokenValueToGppgToken(oper);

            // можно сравнивать только integer real и boolean
            if (!((leftType == "integer" || leftType == "real" || rightType == "boolean") && (rightType == "integer" || rightType == "real" || rightType == "boolean")))
            {
                throw new InvalidOperationException(
                    $"The {operType} opertaion is defined only for integers and reals, {leftType} and {rightType} encountered at {oper.Location}"
                    );
            }

            // Если один из операндов boolean а второй нет
            if ((leftType == "boolean" && rightType != "boolean") || (leftType != "boolean" && rightType == "boolean"))
            {
                throw new InvalidOperationException(
                    $"Boolean can be compared only with boolean, {leftType} and {rightType} encountered at {oper.Location}"
                    );
            }

            switch (operType)
            {
                case Parser.Tokens.LESS:
                    {
                        if (leftType == "boolean" || rightType == "boolean")
                            {
                                throw new InvalidOperationException(
                                    $"The {operType} opertaion is defined only for integers and reals, {leftType} and {rightType} encountered at {oper.Location}"
                                    );
                            }
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {   
                            
                            var val = double.Parse(leftComputed.Value) < double.Parse(rightComputed.Value);
                            var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

                            node.Children = new List<Node> { computed };
                        }
                        break;
                    }
                case Parser.Tokens.LESS_EQUAL:
                    {
                        if (leftType == "boolean" || rightType == "boolean")
                            {
                                throw new InvalidOperationException(
                                    $"The {operType} opertaion is defined only for integers and reals, {leftType} and {rightType} encountered at {oper.Location}"
                                    );
                            }
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {   
                
                            var val = double.Parse(leftComputed.Value) <= double.Parse(rightComputed.Value);
                            var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

                            node.Children = new List<Node> { computed };
                        }
                        break;
                    }
                case Parser.Tokens.GREATER:
                    {
                        if (leftType == "boolean" || rightType == "boolean")
                            {
                                throw new InvalidOperationException(
                                    $"The {operType} opertaion is defined only for integers and reals, {leftType} and {rightType} encountered at {oper.Location}"
                                    );
                            }
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {   
                            var val = double.Parse(leftComputed.Value) > double.Parse(rightComputed.Value);
                            var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

                            node.Children = new List<Node> { computed };
                        }
                        break;
                    }
                case Parser.Tokens.GREATER_EQUAL:
                    {
                        if (leftType == "boolean" || rightType == "boolean")
                            {
                                throw new InvalidOperationException(
                                    $"The {operType} opertaion is defined only for integers and reals, {leftType} and {rightType} encountered at {oper.Location}"
                                    );
                            }
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {   
                            
                            var val = double.Parse(leftComputed.Value) >= double.Parse(rightComputed.Value);
                            var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

                            node.Children = new List<Node> { computed };
                        }
                        break;
                    }
                case Parser.Tokens.EQUAL:
                    {
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {   
                            if (leftComputed.ValueType == "boolean" && rightComputed.ValueType == "boolean")
                            {
                                var val = bool.Parse(leftComputed.Value) == bool.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

                                node.Children = new List<Node> { computed };
                            }
                            else
                            {
                                var val = double.Parse(leftComputed.Value) == double.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

                                node.Children = new List<Node> { computed };
                            }
                        }
                        break;
                    }
                case Parser.Tokens.NOT_EQUAL:
                    {
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {   
                            if (leftComputed.ValueType == "boolean" && rightComputed.ValueType == "boolean")
                            {
                                var val = bool.Parse(leftComputed.Value) != bool.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

                                node.Children = new List<Node> { computed };
                            }
                            else
                            {
                                var val = double.Parse(leftComputed.Value) != double.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "boolean", val.ToString());

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
        else // Summand OPERATOR Simple
        {
            var left = (ComputedExpression)node.Children[1];
            var right = (ComputedExpression)node.Children[2];
            var operToken = node.Children[0].Token!;

            switch (Scanner.Scanner.TokenValueToGppgToken(operToken))
            {
                case Parser.Tokens.LESS:
                    {
                        if (left.ValueType == "boolean" || right.ValueType == "boolean")
                        {
                            throw new InvalidOperationException(
                                $"The {operToken} opertaion is defined only for integers and reals, {left.ValueType} and {right.ValueType} encountered at {operToken.Location}"
                                );
                        }
                        var computed = new ComputedExpression(NodeType.Relation, null, "boolean", null)
                        {
                            Children = node.Children
                        };
                        return computed;
                    }
                case Parser.Tokens.LESS_EQUAL:
                    {
                        if (left.ValueType == "boolean" || right.ValueType == "boolean")
                        {
                            throw new InvalidOperationException(
                                $"The {operToken} opertaion is defined only for integers and reals, {left.ValueType} and {right.ValueType} encountered at {operToken.Location}"
                                );
                        }
                        var computed = new ComputedExpression(NodeType.Relation, null, "boolean", null)
                        {
                            Children = node.Children
                        };
                        return computed;
                    }
                case Parser.Tokens.GREATER:
                    {
                        if (left.ValueType == "boolean" || right.ValueType == "boolean")
                        {
                            throw new InvalidOperationException(
                                $"The {operToken} opertaion is defined only for integers and reals, {left.ValueType} and {right.ValueType} encountered at {operToken.Location}"
                                );
                        }
                        var computed = new ComputedExpression(NodeType.Relation, null, "boolean", null)
                        {
                            Children = node.Children
                        };
                        return computed;
                    }
                case Parser.Tokens.GREATER_EQUAL:
                    {
                        if (left.ValueType == "boolean" || right.ValueType == "boolean")
                        {
                            throw new InvalidOperationException(
                                $"The {operToken} opertaion is defined only for integers and reals, {left.ValueType} and {right.ValueType} encountered at {operToken.Location}"
                                );
                        }
                        var computed = new ComputedExpression(NodeType.Relation, null, "boolean", null)
                        {
                            Children = node.Children
                        };
                        return computed;
                    }
                case Parser.Tokens.EQUAL:
                    {
                        var computed = new ComputedExpression(NodeType.Relation, null, "boolean", null)
                        {
                            Children = node.Children
                        };
                        return computed;
                    }
                case Parser.Tokens.NOT_EQUAL:
                    {
                        var computed = new ComputedExpression(NodeType.Relation, null, "boolean", null)
                        {
                            Children = node.Children
                        };
                        return computed;
                    }
                default:
                    throw new InvalidOperationException($"Undefined operation {operToken} at {operToken.Location}");
            }
        }
    }
}