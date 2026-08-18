using System.Collections.Immutable;

namespace SourceLib.Core.Formats.KeyValues;

public sealed class KeyValuesFormatParser : ITextFormatParser<KeyValuesDocument>
{
    public KeyValuesDocument Parse(ReadOnlySpan<char> input)
    {
        var body = new List<KeyValuePair>();
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

    private KeyValuePair ParsePairOrObject(Lexer lexer, string key)
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
            throw new Exception($"Unexpected token '{next.Value}'");
        }
    }

    private KeyValuePair ParseObject(Lexer lexer, string key, IReadOnlyList<string> tags)
    {
        lexer.NextToken(); // let's eat up the {

        var pairList = new List<KeyValuePair>();
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
                throw new Exception($"Unexpected token in object '{nextToken.Value}'");
            }
        }

        return new KeyValuePair(key, KeyValueValue.FromObject(pairList.ToImmutableList()), tags);
    }

    private KeyValuePair ParsePair(Lexer lexer, string key, IReadOnlyList<string> tags)
    {
        var token = lexer.NextToken();
        var trailingTags = ParseTags(lexer);
        var allTags = tags.Concat(trailingTags).ToImmutableList();

        return new KeyValuePair(
            key,
            KeyValueValue.FromPrimitive(InferPrimitiveFromString(token.Value)),
            allTags
        );
    }

    private ImmutableList<string> ParseTags(Lexer lexer)
    {
        var tags = new List<string>();

        while (lexer.PeekNextToken().Tag == LexerTag.LBracket)
        {
            lexer.NextToken(); // [

            while (lexer.PeekNextToken().Tag != LexerTag.RBracket)
            {
                tags.Add(lexer.NextToken().Value);
            }

            lexer.NextToken(); // ]
        }

        return tags.ToImmutableList();
    }

    private ValuePrimitive InferPrimitiveFromString(string value)
    {
        if (value.Length == 0)
        {
            return ValuePrimitive.FromString(value);
        }

        if (double.TryParse(value, out var floatValue))
        {
            return ValuePrimitive.FromFloat(floatValue);
        }

        if (long.TryParse(value, out var longValue))
        {
            return ValuePrimitive.FromInteger(longValue);
        }

        if (bool.TryParse(value, out var boolValue))
        {
            return ValuePrimitive.FromBoolean(boolValue);
        }

        return ValuePrimitive.FromString(value);
    }
}
