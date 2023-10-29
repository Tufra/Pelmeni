using ConsoleTree;
using PelmeniCompilers.Parser;
using PelmeniCompilers.Models;
using PelmeniCompilers.ShiftReduceParser;

namespace PelmeniCompilers;

internal static class Program
{
    private static async Task Main(string[] args)
    {
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