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
    public void Test_Roundtrips_V0()
    {
        var directoryPath = TestFixtures.GetPath("vpk", "v0", "sourcelib-vpk_dir.vpk");
        var chunkPath = TestFixtures.GetPath("vpk", "v0", "sourcelib-vpk_000.vpk");

        var parser = new VPKFormatParser();
        var serializer = new VPKFormatSerializer();

        var directory = File.ReadAllBytes(directoryPath);
        var chunk = new VPKChunkStream(File.ReadAllBytes(chunkPath));

        var vpk = parser.Parse(directory, [chunk]);

        Assert.Equal(4, vpk.Files.Count);

        var folderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");
        var otherFolderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        Assert.Equal(folderizedFile.Crc, otherFolderizedFile.Crc);

        var textFile = vpk.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal("39", vpk.ReadFileAsString("file_with_number.txt").Trim());

        vpk.WriteFileText("file_with_number.txt", "42");

        var serializedDirectory = serializer.Serialize(vpk);

        var reparsed = parser.Parse(serializedDirectory, [new VPKChunkStream(chunk.ToArray())]);

        Assert.Equal(4, reparsed.Files.Count);

        var modifiedFile = reparsed.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal(textFile.Crc, modifiedFile.Crc);
        Assert.Equal("42", reparsed.ReadFileAsString("file_with_number.txt").Trim());
    }

    [Fact]
    public void Test_Roundtrips_V1()
    {
        var directoryPath = TestFixtures.GetPath("vpk", "v1", "sourcelib-vpk_dir.vpk");
        var chunkPath = TestFixtures.GetPath("vpk", "v1", "sourcelib-vpk_000.vpk");

        var parser = new VPKFormatParser();
        var serializer = new VPKFormatSerializer();

        var directory = File.ReadAllBytes(directoryPath);
        var chunk = new VPKChunkStream(File.ReadAllBytes(chunkPath));

        var vpk = parser.Parse(directory, [chunk]);

        Assert.Equal(4, vpk.Files.Count);

        var folderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");
        var otherFolderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        Assert.Equal(folderizedFile.Crc, otherFolderizedFile.Crc);

        var textFile = vpk.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal("39", vpk.ReadFileAsString("file_with_number.txt").Trim());

        vpk.WriteFileText("file_with_number.txt", "42");

        var serializedDirectory = serializer.Serialize(vpk);

        var reparsed = parser.Parse(serializedDirectory, [new VPKChunkStream(chunk.ToArray())]);

        Assert.Equal(4, reparsed.Files.Count);

        var modifiedFile = reparsed.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal(textFile.Crc, modifiedFile.Crc);
        Assert.Equal("42", reparsed.ReadFileAsString("file_with_number.txt").Trim());
    }

    [Fact]
    public void Test_Roundtrips_HL2_Textures_V2()
    {
        var hl2 = _games.Get(GameId.HalfLife2);

        var directoryPath = hl2.GetPath("hl2", "hl2_misc_dir.vpk");
        var directory = File.ReadAllBytes(directoryPath);

        var chunkPaths = new[]
        {
            hl2.GetPath("hl2", "hl2_misc_000.vpk"),
            hl2.GetPath("hl2", "hl2_misc_001.vpk"),
            hl2.GetPath("hl2", "hl2_misc_002.vpk"),
            hl2.GetPath("hl2", "hl2_misc_003.vpk"),
        };

        var chunks = chunkPaths
            .Select(path => new VPKChunkStream(File.ReadAllBytes(path)))
            .ToList();

        var parser = new VPKFormatParser();
        var serializer = new VPKFormatSerializer();

        var vpk = parser.Parse(directory, chunks);

        Assert.Equal(4, vpk.Chunks.Count);
        Assert.NotNull(vpk.Header);
        Assert.Equal(VPKVersion.v2, vpk.Header.Version);

        Assert.NotEmpty(vpk.Header.ArchiveMD5Section);
        Assert.NotEmpty(vpk.Header.OtherMD5Section);
        Assert.NotEmpty(vpk.Header.SignatureSection);

        var serializedDirectory = serializer.Serialize(vpk);
        var reparsed = parser.Parse(serializedDirectory, chunks);

        Assert.Equal(vpk.Files.Count, reparsed.Files.Count);
        Assert.Equal(vpk.Header.Version, reparsed.Header!.Version);
        Assert.Equal(vpk.Header.ArchiveMD5Section, reparsed.Header.ArchiveMD5Section);
        Assert.Equal(vpk.Header.OtherMD5Section, reparsed.Header.OtherMD5Section);
        Assert.Equal(vpk.Header.SignatureSection, reparsed.Header.SignatureSection);
    }

    [Fact]
    public void Test_Roundtrips_V2()
    {
        var directoryPath = TestFixtures.GetPath("vpk", "v2", "sourcelib-vpk_dir.vpk");
        var chunkPath = TestFixtures.GetPath("vpk", "v2", "sourcelib-vpk_000.vpk");

        var parser = new VPKFormatParser();
        var serializer = new VPKFormatSerializer();

        var directory = File.ReadAllBytes(directoryPath);
        var chunk = new VPKChunkStream(File.ReadAllBytes(chunkPath));

        var vpk = parser.Parse(directory, [chunk]);

        Assert.Equal(4, vpk.Files.Count);

        var folderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");
        var otherFolderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        Assert.Equal(folderizedFile.Crc, otherFolderizedFile.Crc);

        var textFile = vpk.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal("39", vpk.ReadFileAsString("file_with_number.txt").Trim());

        vpk.WriteFileText("file_with_number.txt", "42");

        var serializedDirectory = serializer.Serialize(vpk);

        var reparsed = parser.Parse(serializedDirectory, [new VPKChunkStream(chunk.ToArray())]);

        Assert.Equal(4, reparsed.Files.Count);

        var modifiedFile = reparsed.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal(textFile.Crc, modifiedFile.Crc);
        Assert.Equal("42", reparsed.ReadFileAsString("file_with_number.txt").Trim());
    }

    [Fact]
    public void Test_Reads_HL2_ValveRC()
    {
        var hl2 = _games.Get(GameId.HalfLife2);

        var directoryPath = hl2.GetPath("hl2", "hl2_misc_dir.vpk");
        var directory = File.ReadAllBytes(directoryPath);

        var chunkPaths = new[]
        {
            hl2.GetPath("hl2", "hl2_misc_000.vpk"),
            hl2.GetPath("hl2", "hl2_misc_001.vpk"),
            hl2.GetPath("hl2", "hl2_misc_002.vpk"),
            hl2.GetPath("hl2", "hl2_misc_003.vpk"),
        };

        var chunks = chunkPaths
            .Select(path => new VPKChunkStream(File.ReadAllBytes(path)))
            .ToList();

        var parser = new VPKFormatParser();

        var vpk = parser.Parse(directory, chunks);

        var valveRcContent = vpk.ReadFileAsString("cfg/valve.rc");

        Assert.Contains("exec autoexec.cfg", valveRcContent);
    }

    [Fact]
    public void Test_Roundtrips_Portal2_Textures_V1()
    {
        var portal = _games.Get(GameId.Portal2);

        var directoryPath = portal.GetPath("portal2", "pak01_dir.vpk");
        var directory = File.ReadAllBytes(directoryPath);

        var chunkPaths = Enumerable
            .Range(0, 176)
            .Select(i => portal.GetPath("portal2", $"pak01_{i:000}.vpk"))
            .ToArray();

        var chunks = chunkPaths
            .Select(path => new VPKChunkStream(File.ReadAllBytes(path)))
            .ToList();

        var parser = new VPKFormatParser();
        var serializer = new VPKFormatSerializer();

        var vpk = parser.Parse(directory, chunks);

        Assert.Equal(176, vpk.Chunks.Count);
        Assert.NotNull(vpk.Header);
        Assert.Equal(VPKVersion.v1, vpk.Header.Version);

        var serializedDirectory = serializer.Serialize(vpk);
        var reparsed = parser.Parse(serializedDirectory, chunks);

        Assert.Equal(vpk.Files.Count, reparsed.Files.Count);
        Assert.Equal(vpk.Header.Version, reparsed.Header!.Version);
    }
}
