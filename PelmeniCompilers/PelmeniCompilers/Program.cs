using System.Text.Json;
using ConsoleTree;
using PelmeniCompilers.CodeGeneration;
using PelmeniCompilers.Models;
using CommandLine;

namespace PelmeniCompilers;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var parser = new CommandLine.Parser(config =>
        {
            config.AutoHelp = true;
            config.HelpWriter = Console.Out;
        });
        await parser
            .ParseArguments<RunCommandLineOptions,
                ScannerCommandLineOptions,
                ParserCommandLineOptions,
                CompileCommandLineOptions,
                Task>(args)
            .MapResult(
                (RunCommandLineOptions opts) => Run(opts),
                (ScannerCommandLineOptions opts) => Scan(opts, true),
                (ParserCommandLineOptions opts) => Parse(opts),
                (CompileCommandLineOptions opts) => Compile(opts),
                errs =>
                {
                    Console.WriteLine(string.Join("\n", errs));
                    return Task.CompletedTask;
                });
    }

    private static async Task Run(RunCommandLineOptions options)
    {
        var scanner = await Scan(options.ScannerCommandLineOptions);
        var tree = await Parse(scanner, options.ParserCommandLineOptions);
        CheckSemantic(tree, options.CompileCommandLineOptions);
        Compile(tree, options.CompileCommandLineOptions);
    }

    private static async Task<Scanner.Scanner> Scan(ScannerCommandLineOptions options, bool independentRun = false)
    {
        var scanner = new Scanner.Scanner();
        await scanner.Scan(options.InputFile);

        if (options.Verbose)
        {
            Console.WriteLine(string.Join("\n", scanner.Tokens));
        }

        if (!options.DryRun && independentRun)
        {
            await using var fileWriter = new StreamWriter(options.OutputFile);
            var json = JsonSerializer.Serialize(scanner.Tokens);
            await fileWriter.WriteAsync(json);
        }

        return scanner;
    }

    private static async Task Parse(ParserCommandLineOptions options)
    {
        using var fileReader = new StreamReader(options.InputFile);
        var tokens = await JsonSerializer.DeserializeAsync<List<Token>>(fileReader.BaseStream);

        if (tokens is null)
        {
            throw new InvalidOperationException($"Something wrong with {options.InputFile} file");
        }

        var scanner = new Scanner.Scanner(tokens);
        var tree = await Parse(scanner, options);

        if (!options.DryRun)
        {
            await using var fileWriter = new StreamWriter(options.OutputFile);
            await fileWriter.WriteAsync(JsonSerializer.Serialize(tree));
        }
    }

    private static async Task<Node> Parse(Scanner.Scanner scanner, ParserCommandLineOptions options)
    {
        var parser = new Parser.Parser(scanner);

        parser.Parse();
        await parser.UnfoldDependencies(scanner.FilePath);

        var tree = parser.MainNode!;

        if (options.Verbose)
        {
            PrintTree(tree);
        }

        return tree;
    }
    
    private static void CheckSemantic(Node tree, CompileCommandLineOptions options)
    {
        var semanticAnalyzer = new SemanticAnalyzer.SemanticAnalyzer(tree);
        semanticAnalyzer.Analyze();

        if (options.Verbose)
        {
            PrintTree(tree);
        }
    }
    
    private static async Task Compile(CompileCommandLineOptions options)
    {
        using var fileReader = new StreamReader(options.InputFile);
        var tree = await JsonSerializer.DeserializeAsync<Node>(fileReader.BaseStream);

        if (tree is null)
        {
            throw new InvalidOperationException($"Something wrong with {options.InputFile} file");
        }

        CheckSemantic(tree, options);
        Compile(tree, options);
    }

    private static void Compile(Node tree, CompileCommandLineOptions options)
    {
        var codeGenerator = new CodeGenerator(tree);
        codeGenerator.GenerateCode(options.OutputFile, options.DryRun);
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