using System.Text;
using System.Text.RegularExpressions;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Parser;
using PelmeniCompilers.ShiftReduceParser;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.Scanner;

public class Scanner : AbstractScanner<Node, LexLocation>
{
    public override LexLocation yylloc { get; set; }

    public List<Token> Tokens { get; } = new();
    
    private readonly StringBuilder _buffer = new();
    private int _lineNumber = 1;
    private int _positionEnd = 1;
    private State _state = State.Free;

    private string FilePath { get; set; } = "";

    private IEnumerator<Token>? _tokensEnumerator;

     public override int yylex()
    {
        if (Tokens.Count == 0)
            throw new InvalidOperationException();

        _tokensEnumerator ??= Tokens.GetEnumerator();

        if (_tokensEnumerator.MoveNext())
        {
            var token = _tokensEnumerator.Current;
            yylloc = token.Location;
            yylval = new Node(NodeType.Token, token);
            return (int)TokenValueToGppgToken(token);
        }


        return (int)Parser.Tokens.EOF;
    }

     public override void yyerror(string format, params object[] args)
     {
         throw new SyntaxParserError($"{format} on {yylloc} in {FilePath}");
     }

     private static Tokens TokenValueToGppgToken(Token token)
    {
        switch (token.Value.ToUpper())
        {
            case var intLiteral when Regex.IsMatch(intLiteral, @"^\d+$"):
                return Parser.Tokens.INTEGER_LITERAL;
            case var realLiteral when Regex.IsMatch(realLiteral, @"^\d+.\d+$"):
                return Parser.Tokens.REAL_LITERAL;
            case var charLiteral when Regex.IsMatch(charLiteral, "@^'.'"):
                return Parser.Tokens.CHAR_LITERAL;
            case var stringLiteral when Regex.IsMatch(stringLiteral, "\".*\""):
                return Parser.Tokens.STRING_LITERAL;
            case "TRUE":
                return Parser.Tokens.TRUE;
            case "FALSE":
                return Parser.Tokens.FALSE;
            case "IF":
                return Parser.Tokens.IF;
            case "FOREACH":
                return Parser.Tokens.FOREACH;
            case "FROM":
                return Parser.Tokens.FROM;
            case "END":
                return Parser.Tokens.END;
            case "FOR":
                return Parser.Tokens.FOR;
            case "LOOP":
                return Parser.Tokens.LOOP;
            case "VAR":
                return Parser.Tokens.VAR;
            case "IS":
                return Parser.Tokens.IS;
            case "TYPE":
                return Parser.Tokens.TYPE;
            case "RECORD":
                return Parser.Tokens.RECORD;
            case "ARRAY":
                return Parser.Tokens.ARRAY;
            case "RETURN":
                return Parser.Tokens.RETURN;
            case "WHILE":
                return Parser.Tokens.WHILE;
            case "IN":
                return Parser.Tokens.IN;
            case "REVERSE":
                return Parser.Tokens.REVERSE;
            case "THEN":
                return Parser.Tokens.THEN;
            case "ELSE":
                return Parser.Tokens.ELSE;
            case "ROUTINE":
                return Parser.Tokens.ROUTINE;
            case "REF":
                return Parser.Tokens.REF;
            case "INTEGER":
                return Parser.Tokens.INTEGER;
            case "REAL":
                return Parser.Tokens.REAL;
            case "CHAR":
                return Parser.Tokens.CHAR;
            case "BOOLEAN":
                return Parser.Tokens.BOOLEAN;
            case "MODULE":
                return Parser.Tokens.MODULE;
            case "USE":
                return Parser.Tokens.USE;
            case "OPERATOR":
                return Parser.Tokens.OPERATOR;
            case ".":
                return Parser.Tokens.DOT;
            case ",":
                return Parser.Tokens.COMMA;
            case ":":
                return Parser.Tokens.COLON;
            case ";":
                return Parser.Tokens.SEMICOLON;
            case ":=":
                return Parser.Tokens.ASSIGNMENT_OP;
            case ")":
                return Parser.Tokens.CLOSE_PARENTHESIS;
            case "(":
                return Parser.Tokens.OPEN_PARENTHESIS;
            case "]":
                return Parser.Tokens.CLOSE_BRACKET;
            case "[":
                return Parser.Tokens.OPEN_BRACKET;
            case "=":
                return Parser.Tokens.EQUAL;
            case "++":
                return Parser.Tokens.INCREMENT;
            case "--":
                return Parser.Tokens.DECREMENT;
            case "-":
                return Parser.Tokens.MINUS;
            case "+":
                return Parser.Tokens.PLUS;
            case "*":
                return Parser.Tokens.MULTIPLY;
            case "/":
                return Parser.Tokens.DIVIDE;
            case "%":
                return Parser.Tokens.MOD;
            case "<=":
                return Parser.Tokens.LESS_EQUAL;
            case ">=":
                return Parser.Tokens.GREATER_EQUAL;
            case "<":
                return Parser.Tokens.LESS;
            case ">":
                return Parser.Tokens.GREATER;
            case "<>":
                return Parser.Tokens.NOT_EQUAL;
            case "NOT":
                return Parser.Tokens.NOT;
            case "AND":
                return Parser.Tokens.AND;
            case "OR":
                return Parser.Tokens.OR;
            case "XOR":
                return Parser.Tokens.XOR;
            case "..":
                return Parser.Tokens.RANGE;
            default:
                return Parser.Tokens.IDENTIFIER;
        }
    }

    public void Scan(string path, string text)
    {
        FilePath = path;
        foreach (var symbol in text)
            Process(symbol);

        UploadToken();
    }

    private void Process(char symbol)
    {
        _state = _state switch
        {
            State.Free => ProcessFreeState(symbol),
            State.Delimiter => ProcessDelimiterState(symbol),
            State.StringLiteral => ProcessStringLiteral(symbol),
            State.CharLiteral => ProcessCharLiteral(symbol),
            State.Operator => ProcessOperator(symbol),
            State.IntegerLiteral => ProcessIntegerLiteral(symbol),
            State.RealLiteral => ProcessRealLiteral(symbol),
            _ => throw new ArgumentOutOfRangeException()
        };

        _positionEnd++;
    }

    private State ProcessFreeState(char symbol)
    {
        switch (symbol)
        {
            case '"':
                UploadToken();
                _buffer.Append(symbol);
                return State.StringLiteral;

            case '\'':
                UploadToken();
                _buffer.Append(symbol);
                return State.CharLiteral;

            case ':':
                UploadToken();
                _buffer.Append(symbol);
                return State.Delimiter;

            case ';':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            case ',':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            case '.':
                UploadToken();
                _buffer.Append(symbol);
                UploadToken();
                return State.Free;

            default:

                if (_buffer.Length == 0 && char.IsDigit(symbol))
                {
                    _buffer.Append(symbol);
                    return State.IntegerLiteral;
                }

                if (Regex.IsMatch(symbol.ToString(), @"[=\+\-\<\>\*\/%]"))
                {
                    UploadToken();
                    _buffer.Append(symbol);
                    return State.Operator;
                }

                if (Regex.IsMatch(symbol.ToString(), @"[\(\)\[\]]"))
                {
                    ProcessParenthesis(symbol);
                    return State.Delimiter;
                }

                if (char.IsWhiteSpace(symbol))
                {
                    ProcessWhiteSpace(symbol);
                    return _state;
                }

                _buffer.Append(symbol);
                return _state;
        }
    }

    private State ProcessDelimiterState(char symbol)
    {
        switch (symbol)
        {
            case '"':
                UploadToken();
                _buffer.Append(symbol);
                return State.StringLiteral;

            case '\'':
                UploadToken();
                _buffer.Append(symbol);
                return State.CharLiteral;

            case ';':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            case ',':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            default:
                if (char.IsLetter(symbol))
                {
                    UploadToken();
                    _buffer.Append(symbol);
                    return State.Free;
                }

                if (char.IsDigit(symbol))
                {
                    UploadToken();
                    _buffer.Append(symbol);
                    return State.IntegerLiteral;
                }

                if (char.IsWhiteSpace(symbol))
                {
                    ProcessWhiteSpace(symbol);
                    return State.Free;
                }

                if (Regex.IsMatch(symbol.ToString(), @"[\(\)\[\]]"))
                {
                    ProcessParenthesis(symbol);
                    return State.Free;
                }

                _buffer.Append(symbol);
                return _state;
        }
    }

    private State ProcessStringLiteral(char symbol)
    {
        switch (symbol)
        {
            case '"':
                _buffer.Append('"');
                UploadToken();
                return State.Free;

            default:
                _buffer.Append(symbol);
                return _state;
        }
    }

    private State ProcessCharLiteral(char symbol)
    {
        switch (symbol)
        {
            case '\'':
                _buffer.Append('\'');
                UploadToken();
                return State.Free;

            default:
                _buffer.Append(symbol);
                return _state;
        }
    }

    private State ProcessOperator(char symbol)
    {
        switch (symbol)
        {
            case ';':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            case '"':
                UploadToken();
                _buffer.Append(symbol);
                return State.StringLiteral;

            case '\'':
                UploadToken();
                _buffer.Append(symbol);
                return State.CharLiteral;

            case ',':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            case var digit when char.IsDigit(digit):
                UploadToken();
                _buffer.Append(symbol);
                return State.IntegerLiteral;

            case var letter when char.IsLetter(letter):
                _buffer.Append(symbol);
                UploadToken();
                return State.Free;

            case var whitespace when char.IsWhiteSpace(whitespace):
                ProcessWhiteSpace(symbol);
                return State.Free;

            case var parenthesis when Regex.IsMatch(parenthesis.ToString(), @"[\(\)\[\]]"):
                ProcessParenthesis(symbol);
                return State.Free;

            default:
                _buffer.Append(symbol);
                return _state;
        }
    }

    private State ProcessIntegerLiteral(char symbol)
    {
        switch (symbol)
        {
            case var whiteSpace when char.IsWhiteSpace(whiteSpace):
                ProcessWhiteSpace(symbol);
                return State.Free;

            case ';':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            case '.':
                UploadToken();
                _buffer.Append(symbol);
                return State.RealLiteral;

            case ',':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            default:
                if (Regex.IsMatch(symbol.ToString(), @"[=\+\-\<\>\*\/%]"))
                {
                    UploadToken();
                    _buffer.Append(symbol);
                    return State.Operator;
                }

                if (Regex.IsMatch(symbol.ToString(), @"[\(\)\[\]]"))
                {
                    ProcessParenthesis(symbol);
                    return State.Free;
                }

                _buffer.Append(symbol);
                return _state;
        }
    }

    private State ProcessRealLiteral(char symbol)
    {
        switch (symbol)
        {
            case var whiteSpace when char.IsWhiteSpace(whiteSpace):
                ProcessWhiteSpace(symbol);
                return State.Free;

            case ';':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            case ',':
                UploadToken();
                _buffer.Append(symbol);
                return State.Free;

            default:
                if (Regex.IsMatch(symbol.ToString(), @"\=\+\-\<\>\*\/\%"))
                {
                    UploadToken();
                    _buffer.Append(symbol);
                    return State.Operator;
                }

                if (Regex.IsMatch(symbol.ToString(), @"[\(\)\[\]]"))
                {
                    ProcessParenthesis(symbol);
                    return State.Free;
                }

                if (_buffer[^1] == '.')
                {
                    ProcessDot();
                    return State.Free;
                }

                _buffer.Append(symbol);
                return _state;
        }
    }

    private void ProcessDot()
    {
        _buffer.Remove(_buffer.Length - 1, 1);
        _positionEnd--;
        UploadToken();
        _buffer.Append("..");
        _positionEnd++;
        UploadToken();
    }

    private void ProcessWhiteSpace(char symbol)
    {
        if (!Regex.IsMatch(symbol.ToString(), @"\s"))
            throw new InvalidOperationException($"Expected whitespace, but got '{symbol}'");

        UploadToken();

        /*Tokens.Add(new()
        {
            Location = new LexLocation(_lineNumber, _positionEnd - symbol.ToString().Length, _lineNumber, _positionEnd),
            TokenType = TokenType.WhiteSpace,
            Value = symbol.ToString().Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r")
        });*/


        if (symbol == '\n')
        {
            _lineNumber++;
            _positionEnd = 1;
        }

        _positionEnd--;
        _buffer.Clear();
    }

    private void ProcessParenthesis(char symbol)
    {
        if (!Regex.IsMatch(symbol.ToString(), @"[\(\)\[\]]"))
            throw new InvalidOperationException($"Expected parenthesis, but got '{symbol}'");

        UploadToken();

        Tokens.Add(new Token
        {
            Location = new LexLocation(_lineNumber, _positionEnd - symbol.ToString().Length, _lineNumber, _positionEnd),
            TokenType = TokenType.Delimiter,
            Value = symbol.ToString()
        });

        _buffer.Clear();
    }

    private void UploadToken()
    {
        var token = _buffer.GetToken(_lineNumber, _positionEnd, FilePath);
        if (token is not null)
        {
            _positionEnd++;
            Tokens.Add(token);
        }

        _buffer.Clear();
    }

    private enum State
    {
        Free,
        StringLiteral,
        Delimiter,
        Operator,
        IntegerLiteral,
        RealLiteral,
        CharLiteral
    }
}