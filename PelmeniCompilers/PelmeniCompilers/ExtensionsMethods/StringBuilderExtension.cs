using System.Text;
using System.Text.RegularExpressions;
using PelmeniCompilers.Models;
using PelmeniCompilers.ShiftReduceParser;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.ExtensionsMethods;

public static class StringBuilderExtension
{
    public static Token? GetToken(this StringBuilder stringBuilder, int lineNumber, int positionEnd)
    {
        var tokenString = stringBuilder.ToString();

        if (string.IsNullOrEmpty(tokenString))
            return null;

        var token = new Token
        {
            Location = new LexLocation(lineNumber, positionEnd - tokenString.Length, lineNumber, positionEnd),
            Value = tokenString,
            TokenType = TokenType.Unrecognized
        };

        if (IsIdentifier(tokenString)) token.TokenType = TokenType.Identifier;

        if (IsKeyWord(tokenString)) token.TokenType = TokenType.KeyWord;

        if (IsType(tokenString)) token.TokenType = TokenType.Type;

        if (IsLiteral(tokenString)) token.TokenType = TokenType.Literal;

        if (IsDelimiter(tokenString)) token.TokenType = TokenType.Delimiter;

        if (IsOperator(tokenString)) token.TokenType = TokenType.Operator;

        if (token.TokenType == TokenType.Unrecognized)
            throw new InvalidOperationException($"Token \"{tokenString}\" with {token.Location} unrecognized");
            // Console.WriteLine($"Token \"{tokenString}\" with {token.Location} unrecognized");

        return token;
    }

    private static bool IsIdentifier(string tokenString)
    {
        return Regex.IsMatch(tokenString, @"^[_\w][\w\d_]*$");
    }

    private static bool IsKeyWord(string tokenString)
    {
        return Constants.KeyWords.Contains(tokenString);
    }

    private static bool IsType(string tokenString)
    {
        return Constants.Types.Contains(tokenString);
    }

    private static bool IsDelimiter(string tokenString)
    {
        return Constants.Delimiters.Contains(tokenString);
    }

    private static bool IsOperator(string tokenString)
    {
        return Constants.Operators.Contains(tokenString);
    }

    private static bool IsLiteral(string tokenString)
    {
        return Regex.IsMatch(tokenString, @"^\d+$") // int
               || Regex.IsMatch(tokenString, @"^\d+.\d+$") // real
               || Regex.IsMatch(tokenString, "@^'.'") // char
               || Regex.IsMatch(tokenString, "\".*\"");
        //string
    }
}