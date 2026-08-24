using System.Data.SqlTypes;
using SourceLib.Core.Formats.VPK;
using SourceLib.Tests.GameData;

namespace SourceLib.Tests.Formats;

[Collection("Game")]
public class VPKFormatTests
{
    private readonly IGameDataProvider _games;

    public VPKFormatTests(GameDataFixture games)
    {
        _games = games.Provider;
    }

    [Fact]
    public void Test_Parses_V0()
    {
        var fixturePath = TestFixtures.GetPath("vpk", "v0", "sourcelib-vpk_dir.vpk");
        var chunkPath = TestFixtures.GetPath("vpk", "v0", "sourcelib-vpk_000.vpk");
        var fixtureContent = File.ReadAllBytes(fixturePath);
        var parser = new VPKFormatParser();
        var vpk = parser.Parse(fixtureContent);

        Assert.Equal(4, vpk.Files.Count);

        var folderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");
        var otherFolderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");
        Assert.Equal(folderizedFile.Crc, otherFolderizedFile.Crc);

        var textFile = vpk.Files.First(f => f.Path == "file_with_number.txt");

        var chunks = new List<Stream>();
        var chunkContent = File.OpenRead(chunkPath);
        chunks.Add(chunkContent);
        var vpkFilesystem = VPKDirectoryFileSystem.FromDirectoryFile(vpk, chunks);
        var textFileStream = vpkFilesystem.Open(textFile.Path);
        using var reader = new StreamReader(textFileStream);
        var firstLine = reader.ReadLine();
        Assert.Equal("39", firstLine);
    }
}
