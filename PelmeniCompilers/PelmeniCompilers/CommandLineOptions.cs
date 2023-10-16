using CommandLine;

namespace PelmeniCompilers;

// ReSharper disable once ClassNeverInstantiated.Global
public class CommandLineOptions
{
    [Option('p', "path", Required = true,
        HelpText = "The path to the program file from which the tokens should be recognized")]
    public string PathToFileToParse { get; set; } = null!;
}