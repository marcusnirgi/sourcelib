using System.Collections.Immutable;

namespace SourceLib.Core.Formats.KeyValues2;

public sealed class KeyValues2FormatParser : ITextFormatParser<KeyValues2Document>
{
    public KeyValues2Document Parse(ReadOnlySpan<char> input)
    {
        var body = new List<KeyValues2Pair>();
        var lexer = new Lexer(input.ToString());
        string? header = null;

        while (!lexer.IsAtEnd())
        {
            var nextToken = lexer.NextToken();

            switch (nextToken.Tag)
            {
                case LexerTag.Header:
                    header = nextToken.Value;
                    break;

                case LexerTag.String:
                    var pair = ParsePairOrObjectOrArray(lexer, nextToken.Value);
                    body.Add(pair);
                    break;
            }
        }

        var document = new KeyValues2Document() { Body = body };

        if (header != null)
        {
            document.Header = header;
        }

        return document;
    }

    public KeyValues2Pair ParsePairOrObjectOrArray(Lexer lexer, string key)
    {
        var nextToken = lexer.PeekNextToken();

        if (nextToken.Tag == LexerTag.LBrace)
        {
            return ParseObject(lexer, key);
        }

        if (nextToken.Tag == LexerTag.String)
        {
            var typeHint = lexer.NextToken().Value;
            var valueToken = lexer.PeekNextToken();

            if (valueToken.Tag == LexerTag.String)
            {
                return ParsePair(lexer, key, typeHint);
            }

            if (valueToken.Tag == LexerTag.LBrace)
            {
                return ParseObject(lexer, key, typeHint);
            }

            if (valueToken.Tag == LexerTag.LBracket)
            {
                return ParseArray(lexer, key, typeHint);
            }

            throw new UnexpectedTokenException(valueToken);
        }

        throw new UnexpectedTokenException(nextToken);
    }

    public KeyValues2Pair ParsePair(Lexer lexer, string key, string typeHint)
    {
        var valueToken = lexer.NextToken();

        return new KeyValues2Pair(
            key,
            KeyValueValue.FromPrimitive(
                KeyValues2PrimitiveConverter.ToPrimitive(typeHint, valueToken.Value)
            ),
            typeHint
        );
    }

    public KeyValues2Pair ParseArray(Lexer lexer, string key, string typeHint)
    {
        lexer.NextToken();

        var values = new List<KeyValues2ArrayValue>();

        while (true)
        {
            var nextToken = lexer.PeekNextToken();

            if (nextToken.Tag == LexerTag.RBracket)
            {
                lexer.NextToken();
                break;
            }

            values.Add(ParseArrayValue(lexer, typeHint));
        }

        return new KeyValues2Pair(
            key,
            KeyValueValue.FromPrimitive(ValuePrimitive.FromString(string.Empty)),
            typeHint,
            null,
            values.ToImmutableList()
        );
    }

    private KeyValues2ArrayValue ParseArrayValue(Lexer lexer, string typeHint)
    {
        var valueToken = lexer.NextToken();

        if (valueToken.Tag != LexerTag.String)
        {
            throw new UnexpectedTokenException(valueToken);
        }

        if (lexer.PeekNextToken().Tag == LexerTag.LBrace)
        {
            return ParseAnonymousObject(lexer, valueToken.Value);
        }

        if (typeHint == KeyValues2TypeHint.ElementArray)
        {
            var actualValue = lexer.NextToken();

            if (actualValue.Tag != LexerTag.String)
            {
                throw new UnexpectedTokenException(actualValue);
            }

            return KeyValues2ArrayValue.FromValue(
                KeyValueValue.FromPrimitive(ValuePrimitive.FromString(actualValue.Value)),
                valueToken.Value
            );
        }

        return KeyValues2ArrayValue.FromValue(
            KeyValueValue.FromPrimitive(ValuePrimitive.FromString(valueToken.Value))
        );
    }

    private KeyValues2ArrayValue ParseAnonymousObject(Lexer lexer, string typeHint)
    {
        lexer.NextToken();

        var pairs = new List<KeyValues2Pair>();

        while (true)
        {
            var nextToken = lexer.NextToken();

            if (nextToken.Tag == LexerTag.RBrace)
            {
                break;
            }

            if (nextToken.Tag != LexerTag.String)
            {
                throw new UnexpectedTokenException(nextToken);
            }

            pairs.Add(ParsePairOrObjectOrArray(lexer, nextToken.Value));
        }

        return KeyValues2ArrayValue.FromValue(
            KeyValueValue.FromPrimitive(ValuePrimitive.FromString(string.Empty)),
            typeHint,
            pairs.ToImmutableList()
        );
    }

    public KeyValues2Pair ParseObject(Lexer lexer, string key, string? typeHint = null)
    {
        lexer.NextToken();

        var pairList = new List<KeyValues2Pair>();

        while (true)
        {
            var nextToken = lexer.NextToken();

            if (nextToken.Tag == LexerTag.RBrace)
            {
                break;
            }

            if (nextToken.Tag == LexerTag.String)
            {
                var pair = ParsePairOrObjectOrArray(lexer, nextToken.Value);

                pairList.Add(pair);
            }
            else
            {
                throw new UnexpectedTokenException(nextToken);
            }
        }

        return new KeyValues2Pair(
            key,
            KeyValueValue.FromPrimitive(ValuePrimitive.FromString(string.Empty)),
            typeHint,
            pairList.ToImmutableList()
        );
    }
}
