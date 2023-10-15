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
        throw new NotImplementedException();
    }

    private Node MakeVariableDeclarations()
    {
        throw new NotImplementedException();
    }

    private Node AddToVariableDeclarations(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeBody()
    {
        throw new NotImplementedException();
    }

    private Node AddSimpleDeclarationToBody(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node AddStatementToBody(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeAssignment(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeIncrement(Node i)
    {
        throw new NotImplementedException();
    }

    private Node MakeDecrement(Node i)
    {
        throw new NotImplementedException();
    }

    private Node MakeRoutineCall(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeRoutineCallParameters()
    {
        throw new NotImplementedException();
    }

    private Node MakeRoutineCallParameters(Node i)
    {
        throw new NotImplementedException();
    }

    private Node MakeExpressions(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeExpressionTail()
    {
        throw new NotImplementedException();
    }

    private Node AddToExpressionTail(Node p0, Node p1)
    {
        throw new NotImplementedException();
    }

    private Node MakeWhileLoop(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeForLoop(Node i, Node i1, Node i2)
    {
        throw new NotImplementedException();
    }

    private Node MakeRange(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeRangeExpression(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeForEachLoop(Node i, Node i1, Node i2)
    {
        throw new NotImplementedException();
    }

    private Node MakeIfStatement(Node i, Node i1, Node i2)
    {
        throw new NotImplementedException();
    }

    private Node MakeExpression(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node AddToExpressionTail(Node p0, Node p1, Node i)
    {
        throw new NotImplementedException();
    }

    private Node MakeRelation(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeRelationTail()
    {
        throw new NotImplementedException();
    }

    private Node MakeRelationTail(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeSimple(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeSimpleTail()
    {
        throw new NotImplementedException();
    }

    private Node AddToSimpleTail(Node p0, Node p1, Node p2)
    {
        throw new NotImplementedException();
    }

    private Node MakeFactor(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeFactorTail()
    {
        throw new NotImplementedException();
    }

    private Node AddToFactorTail(Node p0, Node p1, Node p2)
    {
        throw new NotImplementedException();
    }

    private Node MakeSummand(Node p0)
    {
        throw new NotImplementedException();
    }

    private Node MakeModifiablePrimary(Node i, Node i1)
    {
        throw new NotImplementedException();
    }

    private Node MakeModifiablePrimaryTail()
    {
        throw new NotImplementedException();
    }

    private Node AddToModifiablePrimaryTail(Node p0, Node p1)
    {
        throw new NotImplementedException();
    }

    private Node MakeMemberAccess(Node i)
    {
        throw new NotImplementedException();
    }

    private Node MakeArrayAccess(Node i)
    {
        throw new NotImplementedException();
    }

    private Node MakeRef(Node node)
    {
        throw new NotImplementedException();
    }
}