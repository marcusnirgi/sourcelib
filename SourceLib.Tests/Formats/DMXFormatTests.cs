using SourceLib.Core.Engine;
using SourceLib.Core.Formats.DMX;
using SourceLib.Tests.GameData;

namespace SourceLib.Tests.Formats;

[Collection("Game")]
public class DmxFormatTests
{
    private readonly IGameDataProvider _games;

    public DmxFormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_Binary1()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_1_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);

        var rootElement = document.Elements.First(e =>
            e.ClassName == "DmElement" && e.Name == "root"
        );

        var colorThings = rootElement.Attributes.First(a => a.Key == "color_things");
        var colors = (EngineArray<EngineColor4>)colorThings.Value;

        Assert.Contains(
            colors.Values,
            p => p.Value.Red == 106 && p.Value.Green == 0 && p.Value.Blue == 255
        );
    }

    [Fact]
    public void Test_Parses_Binary2()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_2_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);

        var rootElement = document.Elements.First(e =>
            e.ClassName == "DmElement" && e.Name == "root"
        );

        var colorThings = rootElement.Attributes.First(a => a.Key == "color_things");
        var colors = (EngineArray<EngineColor4>)colorThings.Value;

        Assert.Contains(
            colors.Values,
            p => p.Value.Red == 106 && p.Value.Green == 0 && p.Value.Blue == 255
        );
    }

    [Fact]
    public void Test_Parses_Binary3()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_3_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);

        var rootElement = document.Elements.First(e =>
            e.ClassName == "DmElement" && e.Name == "root"
        );

        var colorThings = rootElement.Attributes.First(a => a.Key == "color_things");
        var colors = (EngineArray<EngineColor4>)colorThings.Value;

        Assert.Contains(
            colors.Values,
            p => p.Value.Red == 106 && p.Value.Green == 0 && p.Value.Blue == 255
        );
    }

    [Fact]
    public void Test_Parses_Binary4()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_4_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);

        var rootElement = document.Elements.First(e =>
            e.ClassName == "DmElement" && e.Name == "root"
        );

        var colorThings = rootElement.Attributes.First(a => a.Key == "color_things");
        var colors = (EngineArray<EngineColor4>)colorThings.Value;

        Assert.Contains(
            colors.Values,
            p => p.Value.Red == 106 && p.Value.Green == 0 && p.Value.Blue == 255
        );
    }

    [Fact]
    public void Test_Parses_Binary5()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_5_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);

        var rootElement = document.Elements.First(e =>
            e.ClassName == "DmElement" && e.Name == "root"
        );

        var colorThings = rootElement.Attributes.First(a => a.Key == "color_things");
        var colors = (EngineArray<EngineColor4>)colorThings.Value;

        Assert.Contains(
            colors.Values,
            p => p.Value.Red == 106 && p.Value.Green == 0 && p.Value.Blue == 255
        );
    }

    [Fact]
    public void Test_Parses_Binary5_Model_18()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_5_model_18.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);

        var rootElement = document.Elements.First(e =>
            e.ClassName == "DmElement" && e.Name == "root"
        );

        var colorThings = rootElement.Attributes.First(a => a.Key == "color_things");
        var colors = (EngineArray<EngineColor4>)colorThings.Value;

        Assert.Contains(
            colors.Values,
            p => p.Value.Red == 106 && p.Value.Green == 0 && p.Value.Blue == 255
        );
    }

    [Fact]
    public void Test_Roundtrips_Binary1_Model_1()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_1_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var serializer = new DmxFormatSerializer();

        var originalDocument = parser.Parse(fixtureContent);
        var serializedBytes = serializer.Serialize(originalDocument);

        Assert.Equal(fixtureContent, serializedBytes);

        var reparsedDocument = parser.Parse(serializedBytes);

        Assert.Equivalent(originalDocument, reparsedDocument);
    }

    [Fact]
    public void Test_Roundtrips_Binary5_Model_18()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_5_model_18.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);

        var parser = new DmxFormatParser();
        var serializer = new DmxFormatSerializer();

        var originalDocument = parser.Parse(fixtureContent);
        var serializedBytes = serializer.Serialize(originalDocument);

        Assert.Equal(fixtureContent, serializedBytes);

        var reparsedDocument = parser.Parse(serializedBytes);

        Assert.Equivalent(originalDocument, reparsedDocument);
    }
}
