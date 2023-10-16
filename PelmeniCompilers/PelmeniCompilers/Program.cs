using PelmeniCompilers.Parser;

namespace PelmeniCompilers;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        using var file = new StreamReader(args[0]);
        var fileContent = await file.ReadToEndAsync();
        RunLexerAnalyzer(fileContent);
    }

    private static void RunLexerAnalyzer(string programContent)
    {
        var stateMachine = new Scanner.Scanner();

        foreach (var symbol in programContent) stateMachine.Process(symbol);

        stateMachine.Flush();

        Console.WriteLine(string.Join("\n", stateMachine.Tokens));

        var code = stateMachine.yylex();
        while (code != 3)
        {
            Console.WriteLine($"{stateMachine.yylval} : {((Tokens)code).ToString()}");
            code = stateMachine.yylex();
        }
    }
}