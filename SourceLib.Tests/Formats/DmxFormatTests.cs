using System.Text.Json;
using System.Text.Json.Serialization;
using SourceLib.Core.Engine;
using SourceLib.Core.Formats.Dmx;
using SourceLib.Core.Formats.KeyValues;
using SourceLib.Tests.GameData;

namespace SourceLib.Core.Formats;

[Collection("Game")]
public class DmxFormatTests
{
    private readonly IGameDataProvider _games;

    public DmxFormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_Simple_Binary1_Model1()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "binary_1_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);
        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);

        var dmElement = document.Elements.FirstOrDefault(e =>
            e.ClassName == "DmElement" && e.Name == "Scene"
        );
        Assert.NotNull(dmElement);

        var dmElementSkeleton = dmElement.Attributes.FirstOrDefault(a => a.Key == "skeleton");
        Assert.NotNull(dmElementSkeleton);
        Assert.NotNull((EngineGuid)dmElementSkeleton.Value);
        Assert.Equal(
            "873c37bb-2e74-4b30-97d5-0ccce446b5e0",
            ((EngineGuid)dmElementSkeleton.Value).Value.ToString()
        );

        var dmeDag = document.Elements.First(e => e.ClassName == "DmeDag");
        var colorThings = dmeDag.Attributes.First(a => a.Key == "color_things");
        var colors = (EngineArray<EngineColor4>)colorThings.Value;
        // check that it has my fav colour
        Assert.Contains(
            colors.Values,
            p => p.Value.Red == 106 && p.Value.Green == 0 && p.Value.Blue == 255
        );
    }

    [Fact]
    public void Test_Parses_Complex_Binary1_Model1()
    {
        var fixturePath = TestFixtures.GetPath("dmx", "citizen_head_binary_1_model_1.dmx");
        var fixtureContent = File.ReadAllBytes(fixturePath);
        var parser = new DmxFormatParser();
        var document = parser.Parse(fixtureContent);
        Console.WriteLine(document);
    }
}
