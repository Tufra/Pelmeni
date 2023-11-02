using PelmeniCompilers.Models;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer.Checkers;

public class FactorChecker : BaseNodeRuleChecker
{
    public override NodeType CheckingNodeType => NodeType.Simple;
    public override void Check(Node node)
    {
        if (node.Children.Count == 1)
        {
            var subexpression = node.Children[0];
            subexpression.CheckSemantic();
            
            ComputedExpression computedSub;
            if (subexpression.Children.Count == 1)
            {
                computedSub = (ComputedExpression)subexpression.Children[0];
            }
            else
            {
                computedSub = (ComputedExpression)subexpression.Children[1];
            }
            
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

            if (!((leftType == "integer" || leftType == "real") && (rightType == "integer" || rightType == "real")))
            {
                throw new InvalidOperationException(
                    $"The {operType} opertaion is defined only for integers and reals, {leftType} and {rightType} encountered at {oper.Location}"
                    );
            }

            switch (operType)
            {
                case Parser.Tokens.PLUS:
                    {
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {   
                            if (leftType == "real" || rightType == "real")
                            {
                                var val = double.Parse(leftComputed.Value) + double.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "real", val.ToString());

                                node.Children = new List<Node> { computed };
                            }
                            else
                            {
                                var val = int.Parse(leftComputed.Value) + int.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "integer", val.ToString());

                                node.Children = new List<Node> { computed };
                            }
                        }
                        break;
                    }
                case Parser.Tokens.MINUS:
                    {
                        if (leftComputed.Value is not null && rightComputed.Value is not null) 
                        {
                            if (leftType == "real" || rightType == "real")
                            {
                                var val = double.Parse(leftComputed.Value) - double.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "real", val.ToString());

                                node.Children = new List<Node> { computed };
                            }
                            else
                            {
                                var val = int.Parse(leftComputed.Value) - int.Parse(rightComputed.Value);
                                var computed = new ComputedExpression(NodeType.Summand, null, "integer", val.ToString());

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