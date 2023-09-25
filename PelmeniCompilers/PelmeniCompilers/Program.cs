using System.Text;
using CommandLine;
using PelmeniCompilers.Constants;
using PelmeniCompilers.Models;

namespace PelmeniCompilers;

internal static class Program
{
    private static void Main(string[] args) =>
        Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsedAsync(async options =>
        {
            using var file = new StreamReader(options.PathToFileToParse);
            var fileContent = await file.ReadToEndAsync();
            RunLexerAnalyzer(fileContent);
        });

    private static void RunLexerAnalyzer(string programContent)
    {
        var tokens = new List<Token>();
        var buffer = new StringBuilder();
        var lineNumber = 1;
        var positionBegin = 1;

        foreach (var symbol in programContent)
        {
            switch (symbol)
            {
                case '\n':
                    tokens.Add(new()
                    {
                        
                    });
                    tokens.Add(new()
                    {
                        Position = new Position(lineNumber, positionBegin), TokenType = TokenTypes.WhiteSpace,
                        Value = "\n"
                    });
                    lineNumber++;
                    positionBegin = 1;
                    break;
                case ' ':

                    break;
            }
        }
    }
}