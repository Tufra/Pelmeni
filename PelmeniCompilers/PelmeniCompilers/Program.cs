using System.Text;
using System.Text.RegularExpressions;
using CommandLine;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers;

internal static class Program
{
    private static void Main(string[] args)
    {
        var fileContent = "";
        Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsedAsync(async options =>
        {
            using var file = new StreamReader(options.PathToFileToParse);
            fileContent = await file.ReadToEndAsync();
        });
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
    }
}