using PelmeniCompilers.Models;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class RelationChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Simple;
    public override void Check(Node node)
    {
        if (node.Children.Count == 1)
        {
            var subexpression = node.Children[0];
            subexpression.CheckSemantic();
            
            ComputedExpression computedSub = (ComputedExpression)subexpression.Children[0];
            
            var computed = new ComputedExpression(
                subexpression.Type, 
                null, 
                computedSub.ValueType, 
                computedSub.Value);

            if (computed.Value is null)
            {
                computed.Children = new List<Node> { subexpression };
            }
            node.Children = new List<Node> { computed };
        }
        else if (node.Children.Count == 2)
        {
            var subexpression = node.Children[0];
            subexpression.CheckSemantic();

            ComputedExpression computedSub = (ComputedExpression)subexpression.Children[0];
            
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
        }
        else
        {
            var leftOperand = node.Children[1];
            var rightOperand = node.Children[2];
            var oper = node.Children[0].Token!;

            leftOperand.CheckSemantic();
            rightOperand.CheckSemantic();
            
            ComputedExpression leftComputed;
            if (leftOperand.Children.Count == 1)
            {
                var subexpression = (ComputedExpression)leftOperand.Children[0];
                var computed = new ComputedExpression(
                    leftOperand.Type, 
                    null, 
                    subexpression.ValueType, 
                    subexpression.Value);

                if (computed.Value is null)
                {
                    computed.Children = leftOperand.Children;
                }
                
                leftComputed = computed;
            }
            else
            {
                var subexpression = (ComputedExpression)leftOperand.Children[1];
                var computed = new ComputedExpression(
                    leftOperand.Type, 
                    null, 
                    subexpression.ValueType, 
                    subexpression.Value);

                if (computed.Value is null)
                {
                    computed.Children = leftOperand.Children;
                }
                
                leftComputed = computed;
            }
            node.Children[1] = leftComputed;

            ComputedExpression rightComputed;
            if (rightOperand.Children.Count == 1)
            {
                var subexpression = (ComputedExpression)rightOperand.Children[0];
                var computed = new ComputedExpression(
                    rightOperand.Type, 
                    null, 
                    subexpression.ValueType, 
                    subexpression.Value);

                if (computed.Value is null)
                {
                    computed.Children = rightOperand.Children;
                }
                
                rightComputed = computed;
            }
            else
            {
                var subexpression = (ComputedExpression)leftOperand.Children[1];
                var computed = new ComputedExpression(
                    rightOperand.Type, 
                    null, 
                    subexpression.ValueType, 
                    subexpression.Value);

                if (computed.Value is null)
                {
                    computed.Children = rightOperand.Children;
                }
                
                rightComputed = computed;
            }
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
}