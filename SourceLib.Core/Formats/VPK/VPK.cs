namespace SourceLib.Core.Formats.VPK;

public sealed class VPK
{
    public VPKHeader? Header { get; init; }
    public IList<VPKFile> Files { get; init; } = [];
    public IList<VPKChunkStream> Chunks { get; init; } = [];

    public VPKFile GetFile(string path) =>
        Files.FirstOrDefault(f => f.Path == path) ?? throw new FileNotFoundException(path);

    public VPKStream Open(string path)
    {
        return GetFile(path).Open(Chunks);
    }

    public VPKStream OpenWrite(string path)
    {
        return GetFile(path).OpenWrite(Chunks);
    }

    public byte[] ReadFileAsBytes(string path)
    {
        var stream = Open(path);
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    public string ReadFileAsString(string path)
    {
        var stream = Open(path);
        using var reader = new StreamReader(stream, leaveOpen: true);
        return reader.ReadToEnd();
    }

    public void WriteFileBytes(string path, byte[] data)
    {
        using var stream = OpenWrite(path);
        stream.Write(data);
    }

    public void WriteFileText(string path, string content)
    {
        using var writer = new StreamWriter(OpenWrite(path));
        writer.Write(content);
    }
}
