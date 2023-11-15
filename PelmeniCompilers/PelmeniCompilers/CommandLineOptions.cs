using CommandLine;

namespace PelmeniCompilers;

// ReSharper disable once ClassNeverInstantiated.Global
public class CommandLineOptions
{
    [Option('o', "output", Required = true,
        HelpText = "Path to locate output file")]
    public string OutputFile { get; set; } = null!;
}