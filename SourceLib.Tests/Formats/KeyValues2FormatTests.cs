using SourceLib.Core.Engine;
using SourceLib.Core.Formats.KeyValues2;
using SourceLib.Tests.GameData;

namespace SourceLib.Core.Formats;

[Collection("Game")]
public class KeyValues2FormatTests
{
    private readonly IGameDataProvider _games;

    public KeyValues2FormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_Complex_DMX()
    {
        var fixturePath = TestFixtures.GetPath("kv2", "citizen_head_text.dmx");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues2FormatParser();
        var document = parser.Parse(fixtureContent);
        Assert.Equal("<!-- dmx encoding keyvalues2 4 format model 22 -->", document.Header);

        var dmeModel = document.Body.FirstOrDefault(pair => pair.Key == "DmeModel");
        Assert.NotNull(dmeModel);
        Assert.NotNull(dmeModel.Object);
        var dmeModelName = dmeModel.Object.FirstOrDefault(pair => pair.Key == "name");
        Assert.NotNull(dmeModelName);
        Assert.Equal(KeyValues2TypeHint.String, dmeModelName.TypeHint);
        Assert.Equal("3DS2021_Citizen_model", ((EngineString)dmeModelName.Value!)!.Value);

        var dmeModelChildrenArray = dmeModel.Object.FirstOrDefault(pair => pair.Key == "children");
        Assert.NotNull(dmeModelChildrenArray);
        Assert.NotNull(dmeModelChildrenArray.Array);
        Assert.Equal(11, dmeModelChildrenArray.Array.Count);

        var dmeModelChild = dmeModelChildrenArray.Array!.FirstOrDefault(child =>
            ((EngineString)child.Value!).Value == "0951077f-9859-8c43-be3a-d65f97b60cd5"
        );
        Assert.NotNull(dmeModelChild);
    }

    [Fact]
    public void Test_Roundtrips_Particle_PCF()
    {
        var fixturePath = TestFixtures.GetPath("kv2", "particle.pcf");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues2FormatParser();
        var document = parser.Parse(fixtureContent);
        var writer = new StringWriter();
        var serializer = new KeyValues2FormatSerializer();
        serializer.Serialize(document, writer);
        var serialized = writer.ToString();
        var reparsedDocument = parser.Parse(serialized);
        Assert.Equivalent(document, reparsedDocument);
    }

    [Fact]
    public void Test_Roundtrips_Complex_DMX()
    {
        var fixturePath = TestFixtures.GetPath("kv2", "citizen_head_text.dmx");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues2FormatParser();
        var document = parser.Parse(fixtureContent);
        var writer = new StringWriter();
        var serializer = new KeyValues2FormatSerializer();
        serializer.Serialize(document, writer);
        var serialized = writer.ToString();
        var reparsedDocument = parser.Parse(serialized);
        Assert.Equivalent(document, reparsedDocument);
    }
}
