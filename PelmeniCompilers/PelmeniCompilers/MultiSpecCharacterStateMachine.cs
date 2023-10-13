using System.Text;
using System.Text.RegularExpressions;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.Values;

namespace PelmeniCompilers;

public class MultiSpecCharacterStateMachine
{
    private readonly StringBuilder _buffer;
    private int _lineNumber = 1;
    private int _positionBegin = 1;
    private State _state = State.Free;

    public List<Token> Tokens { get; private set; } = new();

    public MultiSpecCharacterStateMachine()
    {
        _buffer = new StringBuilder();
    }

    public void Process(char symbol)
    {
        _state = _state switch
        {
            State.Free => ProcessFreeState(symbol),
            State.Delimiter => ProcessDelimiterState(symbol),
            State.StringLiteral => ProcessStringLiteral(symbol),
            State.CharLiteral => ProcessCharLiteral(symbol),
            State.Operator => ProcessOperator(symbol),
            State.IntegerLiteral => ProcessIntegerLiteral(symbol),
            State.RealLiteral => ProcessRealLiteral(symbol)
        };

        _positionBegin++;
    }
    
    public void Flush()
    {
       UploadToken();
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
                if (Regex.IsMatch(symbol.ToString(), @"[=\+-<>*/%\.]"))
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

    private void ProcessWhiteSpace(char symbol)
    {
        if (!Regex.IsMatch(symbol.ToString(), @"\s"))
            throw new InvalidOperationException($"Expected whitespace, but got '{symbol}'");

        UploadToken();

        Tokens.Add(new()
        {
            Position = new Position(_lineNumber, _positionBegin),
            TokenType = TokenType.WhiteSpace,
            Value = symbol.ToString().Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r")
        });

        if (symbol == '\n')
        {
            _lineNumber++;
            _positionBegin = 1;
        }

        _buffer.Clear();
    }

    private void ProcessParenthesis(char symbol)
    {
        if (!Regex.IsMatch(symbol.ToString(), @"[\(\)\[\]]"))
            throw new InvalidOperationException($"Expected parenthesis, but got '{symbol}'");

        UploadToken();

        Tokens.Add(new()
        {
            Position = new Position(_lineNumber, _positionBegin),
            TokenType = TokenType.Delimiter,
            Value = symbol.ToString()
        });

        _buffer.Clear();
    }

    private void UploadToken()
    {
        var token = _buffer.GetToken(new Position(_lineNumber, _positionBegin));
        if (token is not null)
            Tokens.Add(token);
        _buffer.Clear();
    }
}

public enum State
{
    Free,
    StringLiteral,
    Delimiter,
    Operator,
    IntegerLiteral,
    RealLiteral,
    CharLiteral
}