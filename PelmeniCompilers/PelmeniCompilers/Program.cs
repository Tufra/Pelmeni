using ConsoleTree;
using PelmeniCompilers.Parser;
using PelmeniCompilers.ShiftReduceParser;

namespace PelmeniCompilers;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        using var file = new StreamReader(args[0]);
        var fileContent = await file.ReadToEndAsync();
        Run(fileContent);
    }

    private static void Run(string programContent)
    {
        var scanner = new Scanner.Scanner();

        scanner.Scan(programContent);

        /*var code = scanner.yylex();
        while (code != 3)
        {
            Console.WriteLine($"{scanner.yylval} : {((Tokens)code).ToString()}");
            code = scanner.yylex();
        }*/

        var parser = new Parser.Parser(scanner);
        try
        {
            parser.Parse();
            Tree.Write(parser.MainNode, (node, _) => Console.Write(node.Type.ToString()),
                (node, _) => node.Children, new DisplaySettings { IndentSize = 2 });
        }
        catch (SyntaxParserError e)
        {
            Console.WriteLine(e);
        }
    }
}