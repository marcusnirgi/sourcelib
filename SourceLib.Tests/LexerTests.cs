using SourceLib.Core;

namespace SourceLib.Tests;

public class LexerTests
{
    private static List<LexerToken> GetAllTokens(Lexer lexer)
    {
        var tokens = new List<LexerToken>();
        while (!lexer.IsAtEnd())
        {
            var token = lexer.NextToken();
            if (token.Tag != LexerTag.EOF)
            {
                tokens.Add(token);
            }
        }

        return tokens;
    }

    [Fact]
    public void Test_Json_WithComments()
    {
        string json = """
            {
                // single line comment here
                "hello": "world",
                /* multi-line
                   comment */
                "array": [1, 2, 3]
            }
            """;

        var lexer = new Lexer(json);
        var tokens = GetAllTokens(lexer);

        Assert.Equal(12, tokens.Count);

        Assert.Equal(LexerTag.LBrace, tokens[0].Tag);
        Assert.Equal(LexerTag.String, tokens[1].Tag);
        Assert.Equal("hello", tokens[1].Value);
        Assert.Equal(LexerTag.Colon, tokens[2].Tag);
        Assert.Equal(LexerTag.String, tokens[3].Tag);
        Assert.Equal("world", tokens[3].Value);
        Assert.Equal(LexerTag.String, tokens[4].Tag);
        Assert.Equal("array", tokens[4].Value);
        Assert.Equal(LexerTag.Colon, tokens[5].Tag);
        Assert.Equal(LexerTag.LBracket, tokens[6].Tag);
        Assert.Equal(LexerTag.String, tokens[7].Tag);
        Assert.Equal("1", tokens[7].Value);
        Assert.Equal(LexerTag.String, tokens[8].Tag);
        Assert.Equal("2", tokens[8].Value);
        Assert.Equal(LexerTag.String, tokens[9].Tag);
        Assert.Equal("3", tokens[9].Value);
        Assert.Equal(LexerTag.RBracket, tokens[10].Tag);
        Assert.Equal(LexerTag.RBrace, tokens[11].Tag);
    }

    [Fact]
    public void Test_KV1_Macros_And_UnquotedStrings()
    {
        string kv1 = """
            #include "test.h"
            "data"
            {
                "hello" "world"
                unquoted_key unquoted_value
            }
            """;

        var lexer = new Lexer(kv1);
        var tokens = GetAllTokens(lexer);

        Assert.Equal(8, tokens.Count);

        Assert.Equal(LexerTag.Macro, tokens[0].Tag);
        Assert.Equal("include \"test.h\"", tokens[0].Value);
        Assert.Equal(LexerTag.String, tokens[1].Tag);
        Assert.Equal("data", tokens[1].Value);
        Assert.Equal(LexerTag.LBrace, tokens[2].Tag);
        Assert.Equal(LexerTag.String, tokens[3].Tag);
        Assert.Equal("hello", tokens[3].Value);
        Assert.Equal(LexerTag.String, tokens[4].Tag);
        Assert.Equal("world", tokens[4].Value);
        Assert.Equal(LexerTag.String, tokens[5].Tag);
        Assert.Equal("unquoted_key", tokens[5].Value);
        Assert.Equal(LexerTag.String, tokens[6].Tag);
        Assert.Equal("unquoted_value", tokens[6].Value);
        Assert.Equal(LexerTag.RBrace, tokens[7].Tag);
    }

    [Fact]
    public void Test_KV2_Headers()
    {
        string kv2 = """
            <!-- DMXVersion keyvalues2_v1 -->
            "CDmeElement"
            {
                "name" "string" "my_element"
            }
            """;

        var lexer = new Lexer(kv2);
        var tokens = GetAllTokens(lexer);

        Assert.Equal(7, tokens.Count);

        Assert.Equal(LexerTag.Header, tokens[0].Tag);
        Assert.Equal("<!-- DMXVersion keyvalues2_v1 -->", tokens[0].Value);
        Assert.Equal(LexerTag.String, tokens[1].Tag);
        Assert.Equal("CDmeElement", tokens[1].Value);
        Assert.Equal(LexerTag.LBrace, tokens[2].Tag);
        Assert.Equal(LexerTag.String, tokens[3].Tag);
        Assert.Equal("name", tokens[3].Value);
        Assert.Equal(LexerTag.String, tokens[4].Tag);
        Assert.Equal("string", tokens[4].Value);
        Assert.Equal(LexerTag.String, tokens[5].Tag);
        Assert.Equal("my_element", tokens[5].Value);
        Assert.Equal(LexerTag.RBrace, tokens[6].Tag);
    }

    [Fact]
    public void Test_KV3_MultilineStrings_And_Equals()
    {
        string kv3 = """"
            <!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->
            {
                boolValue = false
                arrayValue = [ 1, 2 ]
                multiline = """
                line1
                line2
                """
            }
            """";

        var lexer = new Lexer(kv3);
        var tokens = GetAllTokens(lexer);

        Assert.Equal(15, tokens.Count);

        Assert.Equal(LexerTag.Header, tokens[0].Tag);
        Assert.StartsWith("<!-- kv3 encoding:text", tokens[0].Value);
        Assert.Equal(LexerTag.LBrace, tokens[1].Tag);
        Assert.Equal(LexerTag.String, tokens[2].Tag);
        Assert.Equal("boolValue", tokens[2].Value);
        Assert.Equal(LexerTag.Equal, tokens[3].Tag);
        Assert.Equal(LexerTag.String, tokens[4].Tag);
        Assert.Equal("false", tokens[4].Value);
        Assert.Equal(LexerTag.String, tokens[5].Tag);
        Assert.Equal("arrayValue", tokens[5].Value);
        Assert.Equal(LexerTag.Equal, tokens[6].Tag);
        Assert.Equal(LexerTag.LBracket, tokens[7].Tag);
        Assert.Equal(LexerTag.String, tokens[8].Tag);
        Assert.Equal("1", tokens[8].Value);
        Assert.Equal(LexerTag.String, tokens[9].Tag);
        Assert.Equal("2", tokens[9].Value);
        Assert.Equal(LexerTag.RBracket, tokens[10].Tag);
        Assert.Equal(LexerTag.String, tokens[11].Tag);
        Assert.Equal("multiline", tokens[11].Value);
        Assert.Equal(LexerTag.Equal, tokens[12].Tag);
        Assert.Equal(LexerTag.String, tokens[13].Tag);
        Assert.Contains("line1", tokens[13].Value);
        Assert.Contains("line2", tokens[13].Value);
        Assert.Equal(LexerTag.RBrace, tokens[14].Tag);
    }
}
