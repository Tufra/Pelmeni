using System.Text;
using System.Text.RegularExpressions;
using CommandLine;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

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
        var stateMachine = new MultiSpecCharacterStateMachine();

        foreach (var symbol in programContent)
        {
            stateMachine.Process(symbol);
        }

        stateMachine.Flush();

        Console.WriteLine(string.Join("\n", stateMachine.Tokens));

        
        while (stateMachine.yylex() != 3)
        {
            Console.WriteLine(stateMachine.yylval);
        } 
    
    }
}