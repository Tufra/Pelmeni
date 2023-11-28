using ConsoleTree;
using PelmeniCompilers.CodeGeneration;
using PelmeniCompilers.Models;
using CommandLine; 

namespace PelmeniCompilers;

internal static class Program
{
    internal static CommandLineOptions CommandLineOptions;
    
    private static async Task Main(string[] args)
    {
        //CommandLineOptions = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args).Value;
        await Run(args[0]);
    }

    private static async Task Run(string path)
    {
        try
        {
            using var file = new StreamReader(path);
            var fileContent = await file.ReadToEndAsync();
            var scanner = new Scanner.Scanner();

            scanner.Scan(path, fileContent);
            

            var parser = new Parser.Parser(scanner);

            parser.Parse();
            parser.UnfoldDependencies(path);

            var tree = parser.MainNode!;

            PrintTree(tree);
            var semanticAnalyzer = new SemanticAnalyzer.SemanticAnalyzer(tree);
            semanticAnalyzer.Analyze();

            Console.WriteLine("\n--------------------------------\n");
            PrintTree(tree);

            var codeGenerator = new CodeGenerator(tree);
            codeGenerator.GenerateCode("");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void PrintTree(Node tree)
    {
        Tree.Write(
            tree,
            (node, _) =>
            {
                if (node == null) return;

                Console.Write(node.Type.ToString());

                if (node.Type == Values.NodeType.Token)
                {
                    Console.Write(" ({0})", node.Token!.Value);
                }
                else if (node is ComputedExpression computedExpression)
                {
                    Console.Write(" ({0}: {1})", computedExpression.Value, computedExpression.ValueType);
                }
            },
            (node, _) => node is not null ? node.Children : new List<Node>() { },
            new DisplaySettings { IndentSize = 2 });
    }
}