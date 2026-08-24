namespace SourceLib.Core.Formats.VPK;

public sealed class VPKDirectoryFileSystem
{
    public VPK DirectoryFile { get; init; }
    public IReadOnlyList<Stream> Chunks { get; init; }

    private VPKDirectoryFileSystem(VPK dirFile, IReadOnlyList<Stream> chunks)
    {
        DirectoryFile = dirFile;
        Chunks = chunks;
    }

    public static VPKDirectoryFileSystem FromDirectoryFile(
        VPK directory,
        IReadOnlyList<Stream> chunks
    )
    {
        return new(directory, chunks);
    }

    public bool Exists(string filePath)
    {
        return DirectoryFile.Files.Any(f => f.Path == filePath);
    }

    public Stream Open(string filePath)
    {
        var file =
            DirectoryFile.Files.FirstOrDefault(f => f.Path == filePath)
            ?? throw new FileNotFoundException(filePath);

        var stream = new MemoryStream();

        if (file.PreloadData.Count > 0)
        {
            stream.Write(file.PreloadData.ToArray());
        }

        foreach (var part in file.Parts)
        {
            var chunk = Chunks[part.FileNumber];

            chunk.Position = part.Offset;

            var buffer = new byte[part.Size];
            var bytesRead = chunk.Read(buffer, 0, buffer.Length);

            if (bytesRead != buffer.Length)
            {
                throw new EndOfStreamException($"Unexpected end of VPK chunk {part.FileNumber}.");
            }

            stream.Write(buffer);
        }

        stream.Position = 0;

        return stream;
    }
}
