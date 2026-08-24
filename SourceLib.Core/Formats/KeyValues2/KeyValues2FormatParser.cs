using System.Collections.Immutable;

namespace SourceLib.Core.Formats.KeyValues2;

public sealed class KeyValues2FormatParser : ITextFormatParser<KeyValues2Document>
{
    public KeyValues2Document Parse(string input)
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
                    body.Add(ParsePairOrObjectOrArray(lexer, nextToken.Value));
                    break;
            }
        }

        if (header == null)
        {
            throw new InvalidDataException("Missing header for keyvalues2 document");
        }

        return new KeyValues2Document { Header = header, Body = body.ToImmutableList() };
    }

    public KeyValues2Pair ParsePairOrObjectOrArray(Lexer lexer, string key)
    {
        var nextToken = lexer.PeekNextToken();

        if (nextToken.Tag == LexerTag.LBrace)
            return ParseObject(lexer, key);

        if (nextToken.Tag == LexerTag.String)
        {
            var typeHint = lexer.NextToken().Value;
            var valueToken = lexer.PeekNextToken();

            if (valueToken.Tag == LexerTag.String)
                return ParsePair(lexer, key, typeHint);

            if (valueToken.Tag == LexerTag.LBrace)
                return ParseObject(lexer, key, typeHint);

            if (valueToken.Tag == LexerTag.LBracket)
                return ParseArray(lexer, key, typeHint);

            throw new UnexpectedTokenException(valueToken);
        }

        throw new UnexpectedTokenException(nextToken);
    }

    public KeyValues2Pair ParsePair(Lexer lexer, string key, string typeHint)
    {
        var valueToken = lexer.NextToken();

        if (valueToken.Tag != LexerTag.String)
            throw new UnexpectedTokenException(valueToken);

        return new KeyValues2Pair(
            key,
            KeyValues2EngineValueConverter.ToPrimitive(typeHint, valueToken.Value),
            typeHint
        );
    }

    public KeyValues2Pair ParseArray(Lexer lexer, string key, string typeHint)
    {
        lexer.NextToken();

        var values = new List<KeyValues2ArrayItem>();

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

        return new KeyValues2Pair(key, null, typeHint, null, values.ToImmutableList());
    }

    private KeyValues2ArrayItem ParseArrayValue(Lexer lexer, string typeHint)
    {
        var valueToken = lexer.NextToken();

        if (valueToken.Tag != LexerTag.String)
            throw new UnexpectedTokenException(valueToken);

        if (lexer.PeekNextToken().Tag == LexerTag.LBrace)
            return ParseAnonymousObject(lexer, valueToken.Value);

        if (typeHint == KeyValues2TypeHint.ElementArray)
        {
            var actualValue = lexer.NextToken();

            if (actualValue.Tag != LexerTag.String)
                throw new UnexpectedTokenException(actualValue);

            return new KeyValues2ArrayItem(
                KeyValues2EngineValueConverter.ToPrimitive(
                    KeyValues2TypeHint.Element,
                    actualValue.Value
                ),
                KeyValues2TypeHint.Element
            );
        }

        var value = KeyValues2EngineValueConverter.ToPrimitive(typeHint[..^6], valueToken.Value);

        return new KeyValues2ArrayItem(value);
    }

    private KeyValues2ArrayItem ParseAnonymousObject(Lexer lexer, string typeHint)
    {
        lexer.NextToken();

        var pairs = new List<KeyValues2Pair>();

        while (true)
        {
            var nextToken = lexer.NextToken();

            if (nextToken.Tag == LexerTag.RBrace)
                break;

            if (nextToken.Tag != LexerTag.String)
                throw new UnexpectedTokenException(nextToken);

            pairs.Add(ParsePairOrObjectOrArray(lexer, nextToken.Value));
        }

        return new KeyValues2ArrayItem(null, typeHint, pairs.ToImmutableList());
    }

    public KeyValues2Pair ParseObject(Lexer lexer, string key, string? typeHint = null)
    {
        lexer.NextToken();

        var pairList = new List<KeyValues2Pair>();

        while (true)
        {
            var nextToken = lexer.NextToken();

            if (nextToken.Tag == LexerTag.RBrace)
                break;

            if (nextToken.Tag != LexerTag.String)
                throw new UnexpectedTokenException(nextToken);

            pairList.Add(ParsePairOrObjectOrArray(lexer, nextToken.Value));
        }

        return new KeyValues2Pair(key, null, typeHint, pairList.ToImmutableList());
    }
}
