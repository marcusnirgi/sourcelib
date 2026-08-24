namespace SourceLib.Core.Formats.VPK;

public sealed class VPKFile
{
    public required string Path { get; set; }

    public uint Crc { get; set; }

    public ushort PreloadSize { get; set; }

    public IList<VPKFilePart> Parts { get; set; } = [];

    public IList<byte> PreloadData { get; set; } = [];

    public VPKStream Open(IList<Stream> chunks)
    {
        return OpenInternal(chunks, writable: false);
    }

    public VPKStream OpenWrite(IList<Stream> chunks)
    {
        return OpenInternal(chunks, writable: true);
    }

    private VPKStream OpenInternal(IList<Stream> chunks, bool writable)
    {
        var stream = new MemoryStream();

        stream.Write(PreloadData.ToArray());

        foreach (var part in Parts)
        {
            if (part.FileNumber >= chunks.Count)
            {
                throw new InvalidDataException(
                    $"VPK file '{Path}' references missing chunk {part.FileNumber}."
                );
            }

            var chunk = chunks[part.FileNumber];

            if (!chunk.CanSeek)
            {
                throw new InvalidDataException($"VPK chunk {part.FileNumber} is not seekable.");
            }

            var position = chunk.Position;

            try
            {
                chunk.Position = part.Offset;

                var buffer = new byte[part.Size];
                chunk.ReadExactly(buffer);

                stream.Write(buffer);
            }
            finally
            {
                chunk.Position = position;
            }
        }

        stream.Position = 0;

        return new VPKStream(stream, this, chunks, writable);
    }
}

public sealed class VPKFilePart
{
    public ushort FileNumber { get; set; }

    public uint Offset { get; set; }

    public uint Size { get; set; }
}
