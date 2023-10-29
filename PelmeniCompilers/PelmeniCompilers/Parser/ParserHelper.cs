using PelmeniCompilers.Models;
using PelmeniCompilers.ShiftReduceParser;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Parser;

public partial class Parser
{
    public Node? MainNode { get; private set; }

    public record DependencyTreeNode
    {
        public DependencyTreeNode(string path, Node? program)
        {
            Path = path;
            Program = program;
        }

        public string Path { get; private set; }
        public Node? Program { get; private set; }
    }

    public List<string>? UsedModules { get; private set; }

    public Parser(AbstractScanner<Node, LexLocation> scanner) : base(scanner)
    {
    }

    public void UnfoldDependencies(string path)
    {
        Queue<DependencyTreeNode> importsQueue = new();
        importsQueue.Enqueue(new DependencyTreeNode(path, MainNode));


        while (importsQueue.Count > 0)
        {
            var node = importsQueue.Dequeue();
            if (node.Program!.Children![0].Type == NodeType.Module)
            {
                var imports = node.Program!.Children![1].Children!;
                Console.WriteLine(imports.ToString());
                for (var i = 0; i < imports.Count; i++)
                {
                    var fileName = imports[i].Token!.Value;
                    var filePath = Path.Join(Path.GetDirectoryName(node.Path),
                        fileName.Substring(1, fileName.Length - 2));
                    using var file = new StreamReader(filePath);
                    var fileContent = file.ReadToEnd();


                    var scanner = new Scanner.Scanner();
                    var parser = new Parser(scanner);

                    scanner.Scan(filePath, fileContent);
                    parser.Parse();

                    var tree = parser.MainNode;
                    string name = fileName;
                    if (tree!.Children![0].Type == NodeType.Module)
                    {
                        name = tree!.Children![0].Children![0].Token!.Value;
                    }

                    if (UsedModules!.Contains(name))
                    {
                        throw new Exception($"Duplicate module: {name} in {filePath}");
                    }

                    UsedModules!.Add(name);

                    imports[i] = tree;

                    importsQueue.Enqueue(new DependencyTreeNode(filePath, imports[i]));
                }
            }
        }
    }

    private Node MakeProgram(Node node1, Node node2)
    {
        var node = new Node(NodeType.Program, new List<Node> { node1, node2 });
        MainNode ??= node;
        UsedModules = new List<string> { node1.Children![0].Token!.Value };
        return node;
    }

    private Node MakeImports()
    {
        var node = new Node(NodeType.Imports, new List<Node> { });
        return node;
    }

    private Node AddToImports(Node to, Node i)
    {
        to.Children!.Add(i);
        return to;
    }

    private Node MakeModule(Node i)
    {
        var node = new Node(NodeType.Module, new List<Node> { i });
        return node;
    }

    private Node AddToProgram(Node program, Node child)
    {
        program.Children!.Add(child);
        return program;
    }

    private Node MakeVariableDeclaration(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.VariableDeclaration, new List<Node> { i, i1, i2 });

        return node;
    }

    private Node MakeTypeDeclaration(Node i, Node i1)
    {
        var node = new Node(NodeType.TypeDeclaration, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeRoutineDeclaration(Node i, Node i1, Node i2, Node i3)
    {
        var node = new Node(NodeType.RoutineDeclaration, new List<Node> { i, i1, i2, i3 });

        return node;
    }

    private Node MakeParameters()
    {
        var node = new Node(NodeType.Parameters, new List<Node>());

        return node;
    }

    private Node MakeParameters(Node i, Node i1)
    {
        var node = new Node(NodeType.Parameters, new List<Node> { i });
        node.Children!.AddRange(i1.Children!);

        return node;
    }

    private Node MakeParametersTail()
    {
        var node = new Node(NodeType.ParametersTail, new List<Node>());

        return node;
    }

    private Node AddToParametersTail(Node i, Node i1)
    {
        i.Children!.Add(i1);
        return i;
    }

    private Node MakeParameterDeclaration(Node i, Node i1)
    {
        var node = new Node(NodeType.ParameterDeclaration, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeArrayType(Node i, Node i1)
    {
        var node = new Node(NodeType.ArrayType, new List<Node> { i, i1 });
        return node;
    }

    private Node AddToCompoundSizeTail(Node node1, Node node2)
    {
        node1.Children!.Add(node2);
        return node1;
    }

    private Node MakeCompoundSizeTail()
    {
        var node = new Node(NodeType.CompoundSizeTail, new List<Node> { });
        return node;
    }

    private Node MakeRecordType(Node i)
    {
        var node = new Node(NodeType.RecordType, i.Children);

        return node;
    }

    private Node MakeRecordVariableDeclarations()
    {
        var node = new Node(NodeType.RecordVariableDeclarations, new List<Node>());

        return node;
    }

    private Node AddToRecordVariableDeclarations(Node i, Node i1)
    {
        i.Children!.Add(i1);
        return i;
    }

    private Node MakeBody()
    {
        var node = new Node(NodeType.Body, new List<Node>());

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
        var node = new Node(NodeType.Assignment, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeIncrement(Node i)
    {
        var node = new Node(NodeType.Increment, new List<Node> { i });

        return node;
    }

    private Node MakeDecrement(Node i)
    {
        var node = new Node(NodeType.Decrement, new List<Node> { i });

        return node;
    }

    private Node MakeRoutineCall(Node i, Node i1)
    {
        var node = new Node(NodeType.RoutineCall, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeRoutineCallParameters()
    {
        var node = new Node(NodeType.RoutineCallParameters, new List<Node>());

        return node;
    }

    private Node MakeRoutineCallParameters(Node i)
    {
        var node = new Node(NodeType.RoutineCallParameters, i.Children);

        return node;
    }

    private Node MakeExpressions(Node i, Node i1)
    {
        var node = new Node(NodeType.Expressions, new List<Node> { i });
        node.Children!.AddRange(i1.Children!);

        return node;
    }

    private Node MakeExpressionTail()
    {
        var node = new Node(NodeType.ExpressionsTail, new List<Node>());

        return node;
    }

    private Node AddToExpressionTail(Node p0, Node p1)
    {
        p0.Children!.Add(p1);
        return p0;
    }

    private Node MakeWhileLoop(Node i, Node i1)
    {
        var node = new Node(NodeType.WhileLoop, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeForLoop(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.ForLoop, new List<Node> { i, i1, i2 });

        return node;
    }

    private Node MakeRange(Node i, Node i1)
    {
        var node = new Node(NodeType.Range, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeRangeExpression(Node i, Node i1)
    {
        var node = new Node(NodeType.RangeExpression, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeForEachLoop(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.ForeachLoop, new List<Node> { i, i1, i2 });

        return node;
    }

    private Node MakeIfStatement(Node i, Node i1, Node i2)
    {
        var node = new Node(NodeType.IfStatement, new List<Node> { i, i1, i2 });

        return node;
    }

    private Node MakeExpression(Node i, Node i1)
    {
        var node = new Node(NodeType.Expression, new List<Node> { i });
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
        var node = new Node(NodeType.Relation, new List<Node> { i });
        node.Children!.AddRange(i1.Children!);

        return node;
    }

    private Node MakeRelationTail()
    {
        var node = new Node(NodeType.RelationTail, new List<Node>());

        return node;
    }

    private Node MakeRelationTail(Node i, Node i1)
    {
        var node = new Node(NodeType.RelationTail, new List<Node> { i, i1 });

        return node;
    }

    private Node MakeSimple(Node i, Node i1)
    {
        var node = new Node(NodeType.Simple, new List<Node> { i });
        node.Children!.AddRange(i1.Children!);

        return node;
    }

    private Node MakeSimpleTail()
    {
        var node = new Node(NodeType.SimpleTail, new List<Node>());

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
        var node = new Node(NodeType.Factor, new List<Node> { i });
        node.Children!.AddRange(i1.Children!);

        return node;
    }

    private Node MakeFactorTail()
    {
        var node = new Node(NodeType.FactorTail, new List<Node>());

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
        var node = new Node(NodeType.Summand, new List<Node> { p0 });

        return node;
    }

    private Node MakeModifiablePrimary(Node i, Node i1)
    {
        var node = new Node(NodeType.ModifiablePrimary, new List<Node> { i });
        node.Children!.AddRange(i1.Children!);

        return node;
    }

    private Node MakeModifiablePrimaryTail()
    {
        var node = new Node(NodeType.ModifiablePrimaryTail, new List<Node>());

        return node;
    }

    private Node AddToModifiablePrimaryTail(Node p0, Node p1)
    {
        p1.Children!.Add(p0);
        return p1;
    }

    private Node MakeMemberAccess(Node i)
    {
        var node = new Node(NodeType.MemberAccess, new List<Node> { i });

        return node;
    }

    private Node MakeArrayAccess(Node i)
    {
        var node = new Node(NodeType.ArrayAccess, new List<Node> { i });

        return node;
    }

    private Node MakeRef(Node type)
    {
        var node = new Node(NodeType.RefType, new List<Node> { type });
        return node;
    }

    private Node MakeReturn(Node i)
    {
        var node = new Node(NodeType.Return, new List<Node> { i });
        return node;
    }

    private Node MakeCompoundSize(Node node1, Node node2)
    {
        var node = new Node(NodeType.CompoundSize, new List<Node> { node1 });
        node.Children!.AddRange(node2.Children!);
        return node;
    }

    private Node MakeArrayType(Node type)
    {
        var node = new Node(NodeType.ArrayType, new List<Node> { type });
        return node;
    }

    private Node AddToIdentifiersTail(Node node1, Node node2)
    {
        node1.Children!.Add(node2);
        return node1;
    }

    private Node MakeIdentifiersTail()
    {
        var node = new Node(NodeType.IdentifiersTail, new List<Node> { });
        return node;
    }

    private Node MakeVariablesDeclaration(Node node1, Node node2, Node node3)
    {
        var node = new Node(NodeType.VariablesDeclaration, new List<Node> { node1 });
        node.Children!.AddRange(node2.Children!);
        node.Children!.Add(node3);
        return node;
    }

    private Node MakeTypeTail(Node i)
    {
        var node = new Node(NodeType.TypeTail, new List<Node> { i });
        return node;
    }

    private Node MakeTypeTail()
    {
        var node = new Node(NodeType.TypeTail, new List<Node> { });
        return node;
    }

    private Node? MakeVariableInitializationTail(Node i)
    {
        var node = new Node(NodeType.VariableInitializationTail, new List<Node> { i });
        return node;
    }

    private Node? MakeVariableInitializationTail()
    {
        var node = new Node(NodeType.VariableInitializationTail, new List<Node> { });
        return node;
    }

    private Node MakeElse(Node i)
    {
        var node = new Node(NodeType.ElseTail, new List<Node> { i });
        return node;
    }

    private Node MakeElse()
    {
        var node = new Node(NodeType.ElseTail, new List<Node> { });
        return node;
    }

    private Node? MakeReverse(Node i)
    {
        var node = new Node(NodeType.Reverse, new List<Node> { i });
        return node;
    }

    private Node? MakeReverse()
    {
        var node = new Node(NodeType.Reverse, new List<Node> { });
        return node;
    }
}