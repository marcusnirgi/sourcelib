using SourceLib.Core.Engine;
using SourceLib.Core.Formats.KeyValues;
using SourceLib.Tests.GameData;

namespace SourceLib.Tests.Formats;

[Collection("Game")]
public class KeyValues1FormatTests
{
    private readonly IGameDataProvider _games;

    public KeyValues1FormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_VDF_With_Macro_And_Comments()
    {
        var fixturePath = TestFixtures.GetPath("kv", "main.vdf");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValuesFormatParser();
        var document = parser.Parse(fixtureContent);

        Assert.Equal(2, document.Body.Count);
        Assert.Single(document.Macros);

        Assert.Equal(
            "Value1",
            ((EngineString)document.Body.First(pair => pair.Key == "Key1").Value!).Value
        );

        var listValue = document.Body.First(pair => pair.Key == "List");
        Assert.Single(listValue.Object!);
        Assert.Equal(
            "InnerValue1",
            ((EngineString)listValue.Object!.First(pair => pair.Key == "InnerKey1").Value!)!.Value
        );
    }

    [Fact]
    public void Test_Parses_ClientScheme_With_Tags()
    {
        var hl2 = _games.Get(GameId.HalfLife2);
        var path = hl2.GetPath("hl2", "resource", "clientscheme.res");
        var parser = new KeyValuesFormatParser();

        var document = parser.Parse(File.ReadAllText(path));
        var scheme = document.Body.FirstOrDefault(p => p.Key == "Scheme");
        Assert.NotNull(scheme);
        var fonts = scheme.Object!.FirstOrDefault(p => p.Key == "Fonts");
        Assert.NotNull(fonts);
        var defaultFont = fonts.Object!.FirstOrDefault(p => p.Key == "Default");
        Assert.NotNull(defaultFont);

        var xboxFont = defaultFont.Object!.FirstOrDefault(p => p.Tags.Contains("$X360"));
        Assert.NotNull(xboxFont);

        var name = xboxFont.Object!.FirstOrDefault(p => p.Key == "name");
        Assert.NotNull(name);

        Assert.Equal("Verdana", ((EngineString)name.Value!)!.Value);
    }

    [Fact]
    public void Test_Roundtrips_VDF_With_Macro()
    {
        var fixturePath = TestFixtures.GetPath("kv", "main.vdf");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValuesFormatParser();
        var originalDocument = parser.Parse(fixtureContent);
        var serializer = new KeyValues1FormatSerializer();

        using var writer = new StringWriter();
        serializer.Serialize(originalDocument, writer);

        var reparsedDocument = parser.Parse(writer.ToString());

        Assert.Equivalent(originalDocument, reparsedDocument);
    }

    [Fact]
    public void Test_Roundtrips_ClientScheme_With_Tags()
    {
        var hl2 = _games.Get(GameId.HalfLife2);
        var path = hl2.GetPath("hl2", "resource", "clientscheme.res");
        var parser = new KeyValuesFormatParser();

        var originalDocument = parser.Parse(File.ReadAllText(path));
        var serializer = new KeyValues1FormatSerializer();

        using var writer = new StringWriter();
        serializer.Serialize(originalDocument, writer);

        var reparsedDocument = parser.Parse(writer.ToString());

        Assert.Equivalent(originalDocument, reparsedDocument);
    }
}
