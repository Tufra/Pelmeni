using QUT.Gppg;

namespace PelmeniCompilers.Parser;

public partial class Parser
{
    public Parser(AbstractScanner<Node, LexLocation> scanner) : base(scanner)
    {
    }

    private Node MakeProgram(Node i)
    {
        var node = new Node(NodeType.Program, new List<Node>() { i });
        return node;
    }

    private Node AddToProgram(Node program, Node child)
    {
        program.Children!.Add(child);
        return program;
    }

    private Node MakeVariableDeclaration(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.VariableDeclaration, new List<Node>() { i, i1, i2 });
        return node;
    }

    private Node MakeTypeDeclaration(Node i, Node i1)
    {
        return new Node(NodeType.TypeDeclaration, new List<Node>() { i, i1 });
    }

    private Node MakeRoutineDeclaration(Node i, Node i1, Node i2, Node i3)
    {
        return new Node(NodeType.RoutineDeclaration, new List<Node>() { i, i1, i2, i3 });
    }

    private Node MakeParameters()
    {
        return new Node(NodeType.Parameters, new List<Node>() { });
    }

    private Node MakeParameters(Node i, Node i1)
    {
        var node = new Node(NodeType.Parameters, new List<Node>() { i });
        node.Children!.AddRange(i1.Children!);
        return node;
    }

    private Node MakeParametersTail()
    {
        var node = new Node(NodeType.ParametersTail, new List<Node>() { });
        return node;
    }

    private Node AddToParametersTail(Node i, Node i1)
    {
        i.Children!.Add(i1);
        return i;
    }

    private Node MakeParameterDeclaration(Node i, Node i1)
    {
        var node = new Node(NodeType.ParameterDeclaration, new List<Node>() { i, i1 });
        return node;
    }

    private Node MakeArrayType(Node i, Node i1)
    {
        var node = new Node(NodeType.ArrayType, new List<Node>() { i, i1 });
        return node;
    }

    private Node MakeRecordType(Node i)
    {
        var node = new Node(NodeType.RecordType, i.Children);
        return node;
    }

    private Node MakeVariableDeclarations()
    {
        var node = new Node(NodeType.RecordVariableDeclarations, new List<Node>() { });
        return node;
    }

    private Node AddToVariableDeclarations(Node i, Node i1)
    {
        i.Children!.Add(i1);
        return i;
    }

    private Node MakeBody()
    {
        var node = new Node(NodeType.Body, new List<Node>() { });
        return node;
    }

    private Node AddSimpleDeclarationToBody(Node i, Node i1)
    {
        i.Children!.Add(i1);
        return i;
    }

    private Node AddStatementToBody(Node i, Node i1)
    {
        i.Children!.Add(i1);
        return i;
    }

    private Node MakeAssignment(Node i, Node i1)
    {
        var node = new Node(NodeType.Assignment, new List<Node>() { i, i1});
        return node;
    }

    private Node MakeIncrement(Node i)
    {
        var node = new Node(NodeType.Increment, new List<Node>() { i });
        return node;
    }

    private Node MakeDecrement(Node i)
    {
        var node = new Node(NodeType.Decrement, new List<Node>() { i });
        return node;
    }

    private Node MakeRoutineCall(Node i, Node i1)
    {
        var node = new Node(NodeType.RoutineCall, new List<Node>() { i, i1 });
        return node;
    }

    private Node MakeRoutineCallParameters()
    {
        var node = new Node(NodeType.RoutineCallParameters, new List<Node>() { });
        return node;
    }

    private Node MakeRoutineCallParameters(Node i)
    {
        var node = new Node(NodeType.RoutineCallParameters, i.Children);
        return node;
    }

    private Node MakeExpressions(Node i, Node i1)
    {
        var node = new Node(NodeType.Expressions, new List<Node>() { i });
        node.Children!.AddRange(i1.Children!);
        return node;
    }

    private Node MakeExpressionTail()
    {
        var node = new Node(NodeType.ExpressionsTail, new List<Node>() { });
        return node;
    }

    private Node AddToExpressionTail(Node p0, Node p1)
    {
        p0.Children!.Add(p1);
        return p0;
    }

    private Node MakeWhileLoop(Node i, Node i1)
    {
        var node = new Node(NodeType.WhileLoop, new List<Node>() { i, i1 });
        return node;
    }

    private Node MakeForLoop(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.ForLoop, new List<Node>() { i, i1, i2 });
        return node;
    }

    private Node MakeRange(Node i, Node i1)
    {
        var node = new Node(NodeType.Range, new List<Node>() { i, i1 });
        return node;
    }

    private Node MakeRangeExpression(Node i, Node i1)
    {
        var node = new Node(NodeType.RangeExpression, new List<Node>() { i, i1 });
        return node;
    }

    private Node MakeForEachLoop(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.ForeachLoop, new List<Node>() { i, i1, i2 });
        return node;
    }

    private Node MakeIfStatement(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.IfStatement, new List<Node>() { i, i1, i2 });
        return node;
    }

    private Node MakeExpression(Node i, Node i1)
    {
        var node = new Node(NodeType.Expression, new List<Node>() { i });
        node.Children!.AddRange(i1.Children!);
        return node;
    }

    private Node AddToExpressionTail(Node p0, Node p1, Node i)
    {
        i.Children!.Add(p0);
        i.Children!.Add(p1);
        return i;
    }

    private Node MakeRelation(Node i, Node i1)
    {
        var node = new Node(NodeType.Relation, new List<Node>() { i });
        node.Children!.AddRange(i1.Children!);
        return node;
    }

    private Node MakeRelationTail()
    {
        var node = new Node(NodeType.RelationTail, new List<Node>() { });
        return node;
    }

    private Node MakeRelationTail(Node i, Node i1)
    {
        var node = new Node(NodeType.RelationTail, new List<Node>() { i, i1 });
        return node;
    }

    private Node MakeSimple(Node i, Node i1)
    {
        var node = new Node(NodeType.Simple, new List<Node>() { i });
        node.Children!.AddRange(i1.Children!);
        return node;
    }

    private Node MakeSimpleTail()
    {
        var node = new Node(NodeType.SimpleTail, new List<Node>() { });
        return node;
    }

    private Node AddToSimpleTail(Node p0, Node p1, Node p2)
    {
        p2.Children!.Add(p0);
        p2.Children!.Add(p1);
        return p2;
    }

    private Node MakeFactor(Node i, Node i1)
    {
        var node = new Node(NodeType.Factor, new List<Node>() { i });
        node.Children!.AddRange(i1.Children!);
        return node;
    }

    private Node MakeFactorTail()
    {
        var node = new Node(NodeType.FactorTail, new List<Node>() { });
        return node;
    }

    private Node AddToFactorTail(Node p0, Node p1, Node p2)
    {
        p2.Children!.Add(p0);
        p2.Children!.Add(p1);
        return p2;
    }

    private Node MakeSummand(Node p0)
    {
        var node = new Node(NodeType.Summand, new List<Node>() { p0 });
        return node;
    }

    private Node MakeModifiablePrimary(Node i, Node i1)
    {
        var node = new Node(NodeType.ModifiablePrimary, new List<Node>() { i });
        node.Children!.AddRange(i1.Children!);
        return node;
    }

    private Node MakeModifiablePrimaryTail()
    {
        var node = new Node(NodeType.ModifiablePrimaryTail, new List<Node>() { });
        return node;
    }

    private Node AddToModifiablePrimaryTail(Node p0, Node p1)
    {
        p1.Children!.Add(p0);
        return p1;
    }

    private Node MakeMemberAccess(Node i)
    {
        var node = new Node(NodeType.MemberAccess, new List<Node>() { i });
        return node;
    }

    private Node MakeArrayAccess(Node i)
    {
        var node = new Node(NodeType.ArrayAccess, new List<Node>() { i });
        return node;
    }

    private Node MakeRef(Node type)
    {
        var node = new Node(NodeType.RefType, new List<Node>() { type });
        return node;
    }
}