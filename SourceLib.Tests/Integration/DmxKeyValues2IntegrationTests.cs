using SourceLib.Core.Formats.DMX;
using SourceLib.Core.Formats.KeyValues2;
using SourceLib.Tests.GameData;

namespace SourceLib.Tests.Integration;

[Collection("Game")]
public class DmxKeyValues2IntegrationTests
{
    private readonly IGameDataProvider _games;

    public DmxKeyValues2IntegrationTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Dmx_Materializes_Binary5_Model_18_To_KeyValues2()
    {
        var dmxFixturePath = TestFixtures.GetPath("dmx", "binary_5_model_18.dmx");
        var dmxFixtureBytes = File.ReadAllBytes(dmxFixturePath);

        var dmxParser = new DmxFormatParser();
        var dmxDocument = dmxParser.Parse(dmxFixtureBytes);

        var dmxToKv2Materializer = new DmxToKeyValues2Materializer();
        var materialized = dmxToKv2Materializer.Materialize(dmxDocument);

        var kv2FixturePath = TestFixtures.GetPath("kv2", "citizen_head_text.dmx");
        var kv2FixtureStr = File.ReadAllText(kv2FixturePath);

        var kv2Parser = new KeyValues2FormatParser();
        var expected = kv2Parser.Parse(kv2FixtureStr);

        Assert.Equivalent(expected, materialized);
    }
}
