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
        var fixturePath = TestFixtures.GetPath("kv2", "citizen_head.dmx");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues2FormatParser();
        var document = parser.Parse(fixtureContent);
        Console.WriteLine(document);
        Assert.Equal("<!-- dmx encoding keyvalues2 4 format model 22 -->", document.Header);

        var dmeModel = document.Body.FirstOrDefault(pair => pair.Key == "DmeModel");
        Assert.NotNull(dmeModel);
        Assert.NotNull(dmeModel.Children);
        var dmeModelName = dmeModel.Children.FirstOrDefault(pair => pair.Key == "name");
        Assert.NotNull(dmeModelName);
        Assert.Equal(KeyValues2TypeHint.String, dmeModelName.TypeHint);
        Assert.Equal("3DS2021_Citizen_model", dmeModelName.Value.String);

        var dmeModelChildrenArray = dmeModel.Children.FirstOrDefault(pair =>
            pair.Key == "children"
        );
        Assert.NotNull(dmeModelChildrenArray);
        Assert.NotNull(dmeModelChildrenArray.Array);
        Assert.Equal(11, dmeModelChildrenArray.Array.Count);

        var dmeModelChild = dmeModelChildrenArray.Array!.FirstOrDefault(child =>
            child.Value.String == "0951077f-9859-8c43-be3a-d65f97b60cd5"
        );
        Assert.NotNull(dmeModelChild);
    }
}
