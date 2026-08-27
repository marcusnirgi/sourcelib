using SourceLib.Core.Formats.MDL;
using SourceLib.Core.Formats.VPK;
using SourceLib.Tests.GameData;

namespace SourceLib.Tests.Formats;

[Collection("Game")]
public class MDLFormatTests
{
    private readonly IGameDataProvider _games;

    public MDLFormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_V44_HL2_Watermelon()
    {
        var hl2 = _games.Get(GameId.HalfLife2);
        var hl2MiscVpk = hl2.GetVPK(
            new VPKFormatParser(),
            ["hl2", "hl2_misc_dir.vpk"],
            [
                hl2.GetPath("hl2", "hl2_misc_000.vpk"),
                hl2.GetPath("hl2", "hl2_misc_001.vpk"),
                hl2.GetPath("hl2", "hl2_misc_002.vpk"),
                hl2.GetPath("hl2", "hl2_misc_003.vpk"),
            ]
        );
        var fixtureBytes = hl2MiscVpk.ReadFileAsBytes("models/props_junk/watermelon01.mdl");
        var mdlParser = new MDLFormatParser();
        var mdl = mdlParser.Parse(fixtureBytes);
        Assert.Equal("props_junk/watermelon01.mdl", mdl.Header.Name);
        Assert.Equal(1037217748, mdl.Header.Checksum);
    }
}