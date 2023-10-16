using ConsoleTree;
using PelmeniCompilers.Parser;
using PelmeniCompilers.Models;
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
        try
        {
            var scanner = new Scanner.Scanner();

            scanner.Scan(programContent);

            // foreach (var i in scanner.Tokens)
            //     Console.WriteLine(i);


            /*var code = scanner.yylex();
            while (code != 3)
            {
                Console.WriteLine($"{scanner.yylval} : {((Tokens)code).ToString()}");
                code = scanner.yylex();
            }*/

            var parser = new Parser.Parser(scanner);

            parser.Parse();
            
            Tree.Write(
                parser.MainNode, 
                (node, _) => 
                {
                    if (node != null) 
                    { 
                        Console.Write(node.Type.ToString());
                        if (node.Type == Values.NodeType.Token)
                        {
                            Console.Write(" ({0})", node.Token!.Value);
                        }
                    }
                },
                (node, _) => 
                {
                    if (node != null && node.Children != null)
                    {
                        return node.Children;
                    }
                    else
                    {
                        return new List<Node>() { };
                    }
                }, 
                new DisplaySettings { IndentSize = 2 });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}