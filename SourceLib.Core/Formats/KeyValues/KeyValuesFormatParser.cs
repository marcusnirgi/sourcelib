using System.Collections.Immutable;

namespace SourceLib.Core.Formats.KeyValues;

public sealed class KeyValuesFormatParser : ITextFormatParser<KeyValuesDocument>
{
    public KeyValuesDocument Parse(ReadOnlySpan<char> input)
    {
        var body = new List<KeyValuesPair>();
        var macros = new List<string>();

        var lexer = new Lexer(input.ToString());
        while (!lexer.IsAtEnd())
        {
            var nextToken = lexer.NextToken();
            switch (nextToken.Tag)
            {
                case LexerTag.String:
                    var pair = ParsePairOrObject(lexer, nextToken.Value);
                    body.Add(pair);
                    break;
                case LexerTag.Macro:
                    macros.Add(nextToken.Value);
                    break;
                case LexerTag.EOF:
                    break;
            }
        }

        return new KeyValuesDocument() { Body = body.ToImmutableList(), Macros = macros };
    }

    private KeyValuesPair ParsePairOrObject(Lexer lexer, string key)
    {
        var tags = ParseTags(lexer);
        var next = lexer.PeekNextToken();
        if (next.Tag == LexerTag.LBrace)
        {
            var objectValue = ParseObject(lexer, key, tags);
            return objectValue;
        }
        else if (next.Tag == LexerTag.String)
        {
            var pairValue = ParsePair(lexer, key, tags);
            return pairValue;
        }
        else
        {
            throw new UnexpectedTokenException(next);
        }
    }

    private KeyValuesPair ParseObject(Lexer lexer, string key, IReadOnlyList<string> tags)
    {
        lexer.NextToken();

        var pairList = new List<KeyValuesPair>();
        while (true)
        {
            var nextToken = lexer.NextToken();
            if (nextToken.Tag == LexerTag.RBrace)
            {
                break;
            }
            else if (nextToken.Tag == LexerTag.String)
            {
                var pairOrObj = ParsePairOrObject(lexer, nextToken.Value);
                pairList.Add(pairOrObj);
            }
            else
            {
                throw new UnexpectedTokenException(nextToken);
            }
        }

        return new KeyValuesPair(
            key,
            ValuePrimitive.FromString(string.Empty),
            tags,
            pairList.ToImmutableList()
        );
    }

    private KeyValuesPair ParsePair(Lexer lexer, string key, IReadOnlyList<string> tags)
    {
        var token = lexer.NextToken();
        var trailingTags = ParseTags(lexer);
        var allTags = tags.Concat(trailingTags).ToImmutableList();

        return new KeyValuesPair(key, ValuePrimitive.InferFromString(token.Value), allTags);
    }

    private ImmutableList<string> ParseTags(Lexer lexer)
    {
        var tags = new List<string>();

        while (lexer.PeekNextToken().Tag == LexerTag.LBracket)
        {
            lexer.NextToken();

            while (lexer.PeekNextToken().Tag != LexerTag.RBracket)
            {
                tags.Add(lexer.NextToken().Value);
            }

            lexer.NextToken();
        }

        return tags.ToImmutableList();
    }
}
