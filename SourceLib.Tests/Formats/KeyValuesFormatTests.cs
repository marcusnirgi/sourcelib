using SourceLib.Core.Formats.KeyValues;
using SourceLib.Tests.GameData;

namespace SourceLib.Core.Formats;

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
        var mainVdfPath = TestFixtures.GetPath("vdf", "main.vdf");
        var mainVdfContent = File.ReadAllText(mainVdfPath);
        var kv1FormatParser = new KeyValuesFormatParser();
        var document = kv1FormatParser.Parse(mainVdfContent);

        Assert.Equal(2, document.Body.Count);
        Assert.Single(document.Macros);

        Assert.Equal(
            "Value1",
            document.Body.FirstOrDefault(pair => pair.Key == "Key1")?.Value?.String
        );

        var listValue = document.Body.FirstOrDefault(pair => pair.Key == "List");
        Assert.NotNull(listValue);
        Assert.Single(listValue.Value.Object!);
        Assert.Equal(
            "InnerValue1",
            listValue.Value.Object!.FirstOrDefault(pair => pair.Key == "InnerKey1")?.Value.String
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
        var fonts = scheme.Value.Object!.FirstOrDefault(p => p.Key == "Fonts");
        Assert.NotNull(fonts);
        var defaultFont = fonts.Value.Object!.FirstOrDefault(p => p.Key == "Default");
        Assert.NotNull(defaultFont);

        var xboxFont = defaultFont.Value.Object!.FirstOrDefault(p => p.Tags.Contains("$X360"));
        Assert.NotNull(xboxFont);

        var name = xboxFont.Value.Object!.FirstOrDefault(p => p.Key == "name");
        Assert.NotNull(name);

        Assert.Equal("Verdana", name.Value.String);
    }

    [Fact]
    public void Test_Roundtrips_VDF_With_Macro()
    {
        var mainVdfPath = TestFixtures.GetPath("vdf", "main.vdf");
        var mainVdfContent = File.ReadAllText(mainVdfPath);
        var kv1FormatParser = new KeyValuesFormatParser();
        var originalDocument = kv1FormatParser.Parse(mainVdfContent);
        var kv1FormatSerializer = new KeyValues1FormatSerializer();
        using var writer = new StringWriter();
        kv1FormatSerializer.Serialize(originalDocument, writer);
        var serializedDocument = writer.ToString();
        var reparsedDocument = kv1FormatParser.Parse(serializedDocument);
        Assert.Equivalent(originalDocument, reparsedDocument);
    }

    [Fact]
    public void Test_RoundTrips_ClientScheme_With_Tags()
    {
        var hl2 = _games.Get(GameId.HalfLife2);
        var path = hl2.GetPath("hl2", "resource", "clientscheme.res");
        var parser = new KeyValuesFormatParser();

        var originalDocument = parser.Parse(File.ReadAllText(path));
        var serializer = new KeyValues1FormatSerializer();
        using var writer = new StringWriter();
        serializer.Serialize(originalDocument, writer);
        var serializedDocument = writer.ToString();
        var reparsedDocument = parser.Parse(serializedDocument);
        Assert.Equivalent(originalDocument, reparsedDocument);
    }
}
