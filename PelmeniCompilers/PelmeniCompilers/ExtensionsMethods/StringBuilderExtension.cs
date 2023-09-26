using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class StringBuilderExtension
{
    public static Token? GetToken(this StringBuilder stringBuilder, Position position)
    {
        var tokenString = stringBuilder.ToString();

        if (string.IsNullOrEmpty(tokenString))
            return null;

        var token = new Token()
        {
            Position = position,
            Value = tokenString,
            TokenType = TokenType.Unrecognized
        };
        
        if (IsIdentifier(tokenString))
        {
            token.TokenType = TokenType.Identifier;
        }

        if (IsKeyWord(tokenString))
        {
            token.TokenType = TokenType.KeyWord;
        }

        if (IsType(tokenString))
        {
            token.TokenType = TokenType.Type;
        }

        if (IsLiteral(tokenString))
        {
            token.TokenType = TokenType.Literal;
        }
        
        if (IsDelimiter(tokenString))
        {
            token.TokenType = TokenType.Delimiter;
        }
        
        if (IsOperator(tokenString))
        {
            token.TokenType = TokenType.Operator;
        }

        if (token.TokenType == TokenType.Unrecognized)
            Console.WriteLine($"Token \"{tokenString}\" with {position} unrecognized");

        return token;
    }

    private static bool IsIdentifier(string tokenString) =>
        Regex.IsMatch(tokenString, @"[_\w][\w\d_]*");
    
    private static bool IsKeyWord(string tokenString) =>
        Constants.KeyWords.Contains(tokenString);

    private static bool IsType(string tokenString) =>
        Constants.Types.Contains(tokenString);

    private static bool IsDelimiter(string tokenString) =>
        Constants.Delimiters.Contains(tokenString);

    private static bool IsOperator(string tokenString) =>
        Constants.Operators.Contains(tokenString);

    private static bool IsLiteral(string tokenString) =>
        Regex.IsMatch(tokenString, @"^\d+$") // int
        || Regex.IsMatch(tokenString, @"^\d+.\d+$") // real
        || Regex.IsMatch(tokenString, "@^'.'") // char
        || Regex.IsMatch(tokenString, "\".*\""); //string
}