using CommandLine;

namespace PelmeniCompilers;

[Verb("run", isDefault: true)]
public class RunCommandLineOptions
{
    /*[Option("dll-output", Default = false)]
    public bool IsDllOutput { get; set; }*/

    [Value(0, Required = true, Default = true, HelpText = "Path to input file")]
    public string InputFile { get; set; } = null!;

    [Option('o', "output", Default = "output.exe", HelpText = "Path to locate output file")]
    public string OutputFile { get; set; } = null!;

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Print outputs after all stages")]
    public bool Verbose { get; set; } = false;

    [Option("print-tokens", Required = false, Default = false, HelpText = "Print tokens after scanner stage")]
    public bool PrintTokens { get; set; } = false;

    [Option("print-lexer-tree", Required = false, Default = false, HelpText = "Print tree after parsing stage")]
    public bool PrintLexerTree { get; set; } = false;

    [Option("print-semantic-tree", Required = false, Default = false,
        HelpText = "Print tree after semantic check stage")]
    public bool PrintSemanticTree { get; set; } = false;

    [Option("dry-run", Required = false, Default = false, HelpText = "Does not create any output files")]
    public bool DryRun { get; set; } = false;

    public ScannerCommandLineOptions ScannerCommandLineOptions => new()
    {
        InputFile = InputFile,
        DryRun = DryRun,
        OutputFile = OutputFile,
        Verbose = PrintTokens
    };

    public ParserCommandLineOptions ParserCommandLineOptions => new()
    {
        InputFile = InputFile,
        DryRun = DryRun,
        OutputFile = OutputFile,
        Verbose = PrintLexerTree
    };

    public CompileCommandLineOptions CompileCommandLineOptions => new()
    {
        InputFile = InputFile,
        DryRun = DryRun,
        OutputFile = OutputFile,
        Verbose = PrintSemanticTree
    };
}

[Verb("scan")]
public class ScannerCommandLineOptions
{
    [Value(0, Required = true, Default = true, HelpText = "Path to input file")]
    public string InputFile { get; set; } = null!;

    [Option('o', "output", Default = "scanner.json", HelpText = "Path to locate output file")]
    public string OutputFile { get; set; } = null!;

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Print tokens after scanner stage")]
    public bool Verbose { get; set; } = false;

    [Option("dry-run", Required = false, Default = false, HelpText = "Does not create any output files")]
    public bool DryRun { get; set; } = false;
}

[Verb("parse")]
public class ParserCommandLineOptions
{
    [Value(0, Required = true, Default = true, HelpText = "Path to input file")]
    public string InputFile { get; set; } = null!;

    [Option('o', "output", Default = "parser.json", HelpText = "Path to locate output file")]
    public string OutputFile { get; set; } = null!;

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Print tree after parsing stage")]
    public bool Verbose { get; set; } = false;

    [Option("dry-run", Required = false, Default = false, HelpText = "Does not create any output files")]
    public bool DryRun { get; set; } = false;

    public RunCommandLineOptions RunCommandLineOptions => new()
    {
        InputFile = InputFile,
        DryRun = DryRun,
        PrintLexerTree = Verbose
    };
}

[Verb("compile")]
public class CompileCommandLineOptions
{
    [Value(0, Required = true, Default = true, HelpText = "Path to input file")]
    public string InputFile { get; set; } = null!;

    [Option('o', "output", Default = "output.exe", HelpText = "Path to locate output file")]
    public string OutputFile { get; set; } = null!;
    
    [Option('v', "verbose", Required = false, Default = false, HelpText = "Print tree after semantic check stage")]
    public bool Verbose { get; set; } = false;

    [Option("dry-run", Required = false, Default = false, HelpText = "Does not create any output files")]
    public bool DryRun { get; set; } = false;

    public RunCommandLineOptions RunCommandLineOptions => new()
    {
        InputFile = InputFile,
        DryRun = DryRun,
        OutputFile = OutputFile
    };
}