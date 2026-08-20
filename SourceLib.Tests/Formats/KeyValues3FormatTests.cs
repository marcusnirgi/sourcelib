using SourceLib.Core.Formats.KeyValues3;
using SourceLib.Tests.GameData;

namespace SourceLib.Core.Formats;

[Collection("Game")]
public class KeyValues3FormatTests
{
    private readonly IGameDataProvider _games;

    public KeyValues3FormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_Example()
    {
        var fixturePath = TestFixtures.GetPath("kv3", "example.kv3");
        var fixtureContent = File.ReadAllText(fixturePath);
        var parser = new KeyValues3FormatParser();
        var document = parser.Parse(fixtureContent);
        Console.WriteLine(document);
    }
}
