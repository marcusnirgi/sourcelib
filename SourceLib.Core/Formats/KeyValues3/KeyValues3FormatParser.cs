using System.Collections.Immutable;

namespace SourceLib.Core.Formats.KeyValues3;

public sealed class KeyValues3FormatParser : ITextFormatParser<KeyValues3Document>
{
    public KeyValues3Document Parse(ReadOnlySpan<char> input)
    {
        var body = new List<KeyValues3Pair>();
        var lexer = new Lexer(input.ToString());
        string? header = null;

        while (!lexer.IsAtEnd())
        {
            var nextToken = lexer.NextToken();

            switch (nextToken.Tag)
            {
                case LexerTag.String:
                    var pair = ParsePair(lexer, nextToken.Value);
                    body.Add(pair);
                    break;
                case LexerTag.Header:
                    header = nextToken.Value;
                    break;
                case LexerTag.EOF:
                    break;
            }
        }

        return new KeyValues3Document()
        {
            Body = body.ToImmutableList(),
            Header = header ?? string.Empty,
        };
    }

    private KeyValues3Pair ParsePair(Lexer lexer, string key)
    {
        var equalSignToken = lexer.NextToken();

        if (equalSignToken.Tag != LexerTag.Equal)
        {
            throw new UnexpectedTokenException(equalSignToken);
        }

        var nextToken = lexer.PeekNextToken();

        switch (nextToken.Tag)
        {
            case LexerTag.String:
            {
                var valueToken = lexer.NextToken();

                if (lexer.PeekNextToken().Tag == LexerTag.Colon)
                {
                    lexer.NextToken();

                    return new KeyValues3Pair(key, ParseSuffixedString(lexer, valueToken.Value));
                }

                return new KeyValues3Pair(key, ValuePrimitive.InferFromString(valueToken.Value));
            }
            case LexerTag.LBrace:
                return ParseObject(lexer, key);

            case LexerTag.LBracket:
                return ParseArray(lexer, key);

            default:
                throw new UnexpectedTokenException(nextToken);
        }
    }

    private ValuePrimitive ParseSuffixedString(Lexer lexer, string prefix)
    {
        var value = lexer.NextToken();

        if (value.Tag != LexerTag.String)
        {
            throw new UnexpectedTokenException(value);
        }

        return ValuePrimitive.FromString($"{prefix}:{value.Value}");
    }

    private KeyValues3Pair ParseArray(Lexer lexer, string key)
    {
        lexer.NextToken();

        var values = new List<KeyValues3ArrayValue>();

        while (true)
        {
            var nextToken = lexer.PeekNextToken();

            if (nextToken.Tag == LexerTag.RBracket)
            {
                lexer.NextToken();
                break;
            }

            if (nextToken.Tag == LexerTag.Comma)
            {
                lexer.NextToken();
                continue;
            }

            switch (nextToken.Tag)
            {
                case LexerTag.String:
                    values.Add(ParseArrayPrimitive(lexer));
                    break;

                case LexerTag.LBracket:
                    values.Add(ParseAnonymousArray(lexer));
                    break;

                case LexerTag.LBrace:
                    values.Add(ParseAnonymousObject(lexer));
                    break;

                default:
                    throw new UnexpectedTokenException(nextToken);
            }
        }

        return new KeyValues3Pair(
            key,
            ValuePrimitive.FromString(string.Empty),
            array: values.ToImmutableList()
        );
    }

    private KeyValues3ArrayValue ParseArrayPrimitive(Lexer lexer)
    {
        var token = lexer.NextToken();

        return KeyValues3ArrayValue.FromValue(ValuePrimitive.InferFromString(token.Value));
    }

    private KeyValues3ArrayValue ParseAnonymousArray(Lexer lexer)
    {
        lexer.NextToken();

        var values = new List<KeyValues3ArrayValue>();

        while (true)
        {
            var nextToken = lexer.PeekNextToken();

            if (nextToken.Tag == LexerTag.RBracket)
            {
                lexer.NextToken();
                break;
            }

            if (nextToken.Tag == LexerTag.Comma)
            {
                lexer.NextToken();
                continue;
            }

            switch (nextToken.Tag)
            {
                case LexerTag.String:
                    values.Add(ParseArrayPrimitive(lexer));
                    break;

                case LexerTag.LBracket:
                    values.Add(ParseAnonymousArray(lexer));
                    break;

                case LexerTag.LBrace:
                    values.Add(ParseAnonymousObject(lexer));
                    break;

                default:
                    throw new UnexpectedTokenException(nextToken);
            }
        }

        return KeyValues3ArrayValue.FromValue(
            ValuePrimitive.FromString(string.Empty),
            array: values.ToImmutableList()
        );
    }

    private KeyValues3ArrayValue ParseAnonymousObject(Lexer lexer)
    {
        lexer.NextToken();

        var pairs = new List<KeyValues3Pair>();

        while (true)
        {
            var nextToken = lexer.NextToken();

            if (nextToken.Tag == LexerTag.RBrace)
            {
                break;
            }

            if (nextToken.Tag == LexerTag.String)
            {
                pairs.Add(ParsePair(lexer, nextToken.Value));
                continue;
            }

            throw new UnexpectedTokenException(nextToken);
        }

        return KeyValues3ArrayValue.FromValue(
            ValuePrimitive.FromString(string.Empty),
            children: pairs.ToImmutableList()
        );
    }

    private KeyValues3Pair ParseObject(Lexer lexer, string key)
    {
        lexer.NextToken();

        var pairs = new List<KeyValues3Pair>();

        while (true)
        {
            var nextToken = lexer.NextToken();

            if (nextToken.Tag == LexerTag.RBrace)
            {
                break;
            }

            if (nextToken.Tag == LexerTag.String)
            {
                pairs.Add(ParsePair(lexer, nextToken.Value));
                continue;
            }

            throw new UnexpectedTokenException(nextToken);
        }

        return new KeyValues3Pair(
            key,
            ValuePrimitive.FromString(string.Empty),
            children: pairs.ToImmutableList()
        );
    }
}
