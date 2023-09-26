namespace PelmeniCompilers.Values;

public static class Constants
{
    public static readonly string[] KeyWords =
    {
        "true", "false", "if", "foreach", "from", "end", "for", "loop", "var", "is", "type", "record", "array", "while",
        "in", "reverse", "then", "else", "routine"
    };

    public static readonly string[] Types =
    {
        "integer", "real", "char", "boolean", "Integer", "Real", "Char", "Boolean"
    };

    public static readonly string[] Delimiters =
    {
        ".", ",", ":", ";", ":=", ")", "(", "]", "["
    };

    public static readonly string[] Operators =
    {
        "=", "++", "--", "-", "+", "*", "/", "%", "<=", ">=", "<", ">", "<>", "and", "or", "not", ".."
    };
}