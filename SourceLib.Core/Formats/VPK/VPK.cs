namespace SourceLib.Core.Formats.VPK;

public sealed class VPK
{
    public VPKHeader? Header { get; init; }
    public IList<VPKFile> Files { get; init; } = [];
    public IList<Stream> Chunks { get; init; } = [];

    public VPKFile GetFile(string path) =>
        Files.FirstOrDefault(f => f.Path == path) ?? throw new FileNotFoundException(path);

    public Stream Open(string path)
    {
        return GetFile(path).Open(Chunks);
    }

    public Stream OpenWrite(string path)
    {
        return GetFile(path).OpenWrite(Chunks);
    }
}
