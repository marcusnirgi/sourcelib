using System.Buffers;
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

        var directory = File.ReadAllBytes(directoryPath);
        var chunk = new MemoryStream();
        chunk.Write(File.ReadAllBytes(chunkPath));
        chunk.Position = 0;

        var parser = new VPKFormatParser();
        var vpk = parser.Parse(directory, [chunk]);

        Assert.Equal(4, vpk.Files.Count);

        var folderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        var otherFolderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        Assert.Equal(folderizedFile.Crc, otherFolderizedFile.Crc);

        var textFile = vpk.Files.First(f => f.Path == "file_with_number.txt");

        using (var reader = new StreamReader(textFile.Open(vpk.Chunks)))
        {
            Assert.Equal("39", reader.ReadLine());
        }

        using (var writer = new StreamWriter(textFile.OpenWrite(vpk.Chunks)))
        {
            writer.WriteLine("42");
        }

        var directoryWriter = new ArrayBufferWriter<byte>();
        var serializer = new VPKFormatSerializer();

        serializer.Serialize(vpk, directoryWriter);

        var serializedDirectory = directoryWriter.WrittenSpan.ToArray();

        chunk.Position = 0;

        var serializedChunk = new byte[chunk.Length];
        chunk.ReadExactly(serializedChunk);

        var reparsedChunk = new MemoryStream(serializedChunk);

        var reparsed = parser.Parse(serializedDirectory, [reparsedChunk]);

        Assert.Equal(4, reparsed.Files.Count);

        var modifiedFile = reparsed.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal(textFile.Crc, modifiedFile.Crc);

        using var modifiedReader = new StreamReader(modifiedFile.Open(reparsed.Chunks));

        Assert.Equal("42", modifiedReader.ReadLine());
    }

    [Fact]
    public void Test_Roundtrips_V1()
    {
        var directoryPath = TestFixtures.GetPath("vpk", "v1", "sourcelib-vpk_dir.vpk");

        var chunkPath = TestFixtures.GetPath("vpk", "v1", "sourcelib-vpk_000.vpk");

        var directory = File.ReadAllBytes(directoryPath);
        var chunk = new MemoryStream();
        chunk.Write(File.ReadAllBytes(chunkPath));
        chunk.Position = 0;

        var parser = new VPKFormatParser();
        var vpk = parser.Parse(directory, [chunk]);

        Assert.Equal(4, vpk.Files.Count);

        var folderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        var otherFolderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        Assert.Equal(folderizedFile.Crc, otherFolderizedFile.Crc);

        var textFile = vpk.Files.First(f => f.Path == "file_with_number.txt");

        using (var reader = new StreamReader(textFile.Open(vpk.Chunks)))
        {
            Assert.Equal("39", reader.ReadLine());
        }

        using (var writer = new StreamWriter(textFile.OpenWrite(vpk.Chunks)))
        {
            writer.WriteLine("42");
        }

        var directoryWriter = new ArrayBufferWriter<byte>();
        var serializer = new VPKFormatSerializer();

        serializer.Serialize(vpk, directoryWriter);

        var serializedDirectory = directoryWriter.WrittenSpan.ToArray();

        chunk.Position = 0;

        var serializedChunk = new byte[chunk.Length];
        chunk.ReadExactly(serializedChunk);

        var reparsedChunk = new MemoryStream(serializedChunk);

        var reparsed = parser.Parse(serializedDirectory, [reparsedChunk]);

        Assert.Equal(4, reparsed.Files.Count);

        var modifiedFile = reparsed.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal(textFile.Crc, modifiedFile.Crc);

        using var modifiedReader = new StreamReader(modifiedFile.Open(reparsed.Chunks));

        Assert.Equal("42", modifiedReader.ReadLine());
    }

    [Fact]
    public void Test_Roundtrips_V2()
    {
        var directoryPath = TestFixtures.GetPath("vpk", "v2", "sourcelib-vpk_dir.vpk");

        var chunkPath = TestFixtures.GetPath("vpk", "v2", "sourcelib-vpk_000.vpk");

        var directory = File.ReadAllBytes(directoryPath);
        var chunk = new MemoryStream();
        chunk.Write(File.ReadAllBytes(chunkPath));
        chunk.Position = 0;

        var parser = new VPKFormatParser();
        var vpk = parser.Parse(directory, [chunk]);

        Assert.Equal(4, vpk.Files.Count);

        var folderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        var otherFolderizedFile = vpk.Files.First(f => f.Path == "folder/folderized.kv2");

        Assert.Equal(folderizedFile.Crc, otherFolderizedFile.Crc);

        var textFile = vpk.Files.First(f => f.Path == "file_with_number.txt");

        using (var reader = new StreamReader(textFile.Open(vpk.Chunks)))
        {
            Assert.Equal("39", reader.ReadLine());
        }

        using (var writer = new StreamWriter(textFile.OpenWrite(vpk.Chunks)))
        {
            writer.WriteLine("42");
        }

        var directoryWriter = new ArrayBufferWriter<byte>();
        var serializer = new VPKFormatSerializer();

        serializer.Serialize(vpk, directoryWriter);

        var serializedDirectory = directoryWriter.WrittenSpan.ToArray();

        chunk.Position = 0;

        var serializedChunk = new byte[chunk.Length];
        chunk.ReadExactly(serializedChunk);

        var reparsedChunk = new MemoryStream(serializedChunk);

        var reparsed = parser.Parse(serializedDirectory, [reparsedChunk]);

        Assert.Equal(4, reparsed.Files.Count);

        var modifiedFile = reparsed.Files.First(f => f.Path == "file_with_number.txt");

        Assert.Equal(textFile.Crc, modifiedFile.Crc);

        using var modifiedReader = new StreamReader(modifiedFile.Open(reparsed.Chunks));

        Assert.Equal("42", modifiedReader.ReadLine());
    }
}
