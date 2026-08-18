namespace SourceLib.Core;

public enum LexerTag
{
    Whitespace,
    LBracket,
    RBracket,
    LBrace,
    RBrace,
    LParen,
    RParen,
    Colon,
    Semicolon,
    Comma,
    String,
    Macro,
    Header,
    Equal,
    EOF,
}

public struct LexerToken
{
    public LexerTag Tag { get; }
    public string Value { get; }

    public LexerToken(string value, LexerTag tag)
    {
        Value = value;
        Tag = tag;
    }
}

public class Lexer
{
    private readonly string _inputText;
    private int _position = 0;
    private LexerToken? _nextToken = null;

    public Lexer(string inputText)
    {
        _inputText = inputText;
    }

    public bool IsAtEnd()
    {
        return !_nextToken.HasValue && _position >= _inputText.Length;
    }

    public LexerToken NextToken()
    {
        if (_nextToken.HasValue)
        {
            var token = _nextToken.Value;
            _nextToken = null;
            return token;
        }

        return InternalNext();
    }

    public LexerToken PeekNextToken()
    {
        if (!_nextToken.HasValue)
        {
            _nextToken = InternalNext();
        }

        return _nextToken.Value;
    }

    private LexerToken InternalNext()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
        {
            return new LexerToken(string.Empty, LexerTag.EOF);
        }

        char currentChar = _inputText[_position];

        switch (currentChar)
        {
            case '{':
                return ConsumeCharToken("{", LexerTag.LBrace);
            case '}':
                return ConsumeCharToken("}", LexerTag.RBrace);
            case '[':
                return ConsumeCharToken("[", LexerTag.LBracket);
            case ']':
                return ConsumeCharToken("]", LexerTag.RBracket);
            case '(':
                return ConsumeCharToken("(", LexerTag.LParen);
            case ')':
                return ConsumeCharToken(")", LexerTag.RParen);
            case '=':
                return ConsumeCharToken("=", LexerTag.Equal);
            case ':':
                return ConsumeCharToken(":", LexerTag.Colon);
            case ';':
                return ConsumeCharToken(";", LexerTag.Semicolon);

            case '#':
                _position++;
                int macroStart = _position;
                SkipToEndOfLine();
                string macroSlice = _inputText
                    .Substring(macroStart, _position - macroStart)
                    .TrimEnd('\r');
                if (_position < _inputText.Length)
                    _position++;
                return new LexerToken(macroSlice, LexerTag.Macro);

            case '<':
                if (MatchAhead("<!--"))
                {
                    int headerStart = _position;
                    SkipToEndOfLine();
                    string headerSlice = _inputText
                        .Substring(headerStart, _position - headerStart)
                        .TrimEnd('\r');
                    if (_position < _inputText.Length)
                        _position++;
                    return new LexerToken(headerSlice, LexerTag.Header);
                }

                return ConsumeUnquotedString();

            case '"':
                if (PeekCharOffset(1) == '"' && PeekCharOffset(2) == '"')
                {
                    return ConsumeMultilineString();
                }

                return ConsumeQuotedString();

            default:
                return ConsumeUnquotedString();
        }
    }

    private LexerToken ConsumeCharToken(string value, LexerTag tag)
    {
        _position++;
        return new LexerToken(value, tag);
    }

    private void SkipWhitespaceAndComments()
    {
        while (_position < _inputText.Length)
        {
            char c = _inputText[_position];

            if (c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == ',')
            {
                _position++;
            }
            else if (c == '/' && _position + 1 < _inputText.Length)
            {
                char nextC = _inputText[_position + 1];
                if (nextC == '/')
                {
                    _position += 2;
                    SkipToEndOfLine();
                }
                else if (nextC == '*')
                {
                    _position += 2;
                    while (_position + 1 < _inputText.Length)
                    {
                        if (_inputText[_position] == '*' && _inputText[_position + 1] == '/')
                        {
                            _position += 2;
                            break;
                        }

                        _position++;
                    }

                    if (
                        _position + 1 >= _inputText.Length
                        && !(_inputText[_position - 1] == '*' && _inputText[_position] == '/')
                    )
                    {
                        _position = _inputText.Length;
                    }
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    private LexerToken ConsumeUnquotedString()
    {
        int start = _position;
        while (_position < _inputText.Length)
        {
            char c = _inputText[_position];
            if (
                c == ' '
                || c == '\t'
                || c == '\n'
                || c == '\r'
                || c == '{'
                || c == '}'
                || c == '['
                || c == ']'
                || c == '('
                || c == ')'
                || c == '"'
                || c == '='
                || c == ':'
                || c == ','
                || c == ';'
            )
            {
                break;
            }

            _position++;
        }

        return new LexerToken(_inputText.Substring(start, _position - start), LexerTag.String);
    }

    private LexerToken ConsumeQuotedString()
    {
        _position++;
        int start = _position;
        bool escaped = false;

        while (_position < _inputText.Length)
        {
            char c = _inputText[_position];
            if (c == '\\' && !escaped)
            {
                escaped = true;
                _position++;
                continue;
            }

            if (c == '"' && !escaped)
            {
                string value = _inputText.Substring(start, _position - start);
                _position++;
                return new LexerToken(value, LexerTag.String);
            }

            escaped = false;
            _position++;
        }

        return new LexerToken(_inputText.Substring(start, _position - start), LexerTag.String);
    }

    private LexerToken ConsumeMultilineString()
    {
        _position += 3;
        int start = _position;

        while (_position + 2 < _inputText.Length)
        {
            if (
                _inputText[_position] == '"'
                && _inputText[_position + 1] == '"'
                && _inputText[_position + 2] == '"'
            )
            {
                string value = _inputText.Substring(start, _position - start);
                _position += 3;
                return new LexerToken(value, LexerTag.String);
            }

            _position++;
        }

        string fallbackValue = _inputText.Substring(start);
        _position = _inputText.Length;
        return new LexerToken(fallbackValue, LexerTag.String);
    }

    public void SkipToEndOfLine()
    {
        while (_position < _inputText.Length && _inputText[_position] != '\n')
        {
            _position++;
        }
    }

    private char PeekCharOffset(int offset)
    {
        if (_position + offset >= _inputText.Length)
            return '\0';
        return _inputText[_position + offset];
    }

    private bool MatchAhead(string target)
    {
        if (_position + target.Length > _inputText.Length)
            return false;

        for (int i = 0; i < target.Length; i++)
        {
            if (_inputText[_position + i] != target[i])
                return false;
        }

        return true;
    }

    public (int cursor, LexerToken? nextToken) Snapshot()
    {
        return (_position, _nextToken);
    }

    public void Restore((int cursor, LexerToken? nextToken) state)
    {
        _position = state.cursor;
        _nextToken = state.nextToken;
    }
}
