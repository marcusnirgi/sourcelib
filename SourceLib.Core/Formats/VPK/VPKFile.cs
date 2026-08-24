namespace SourceLib.Core.Formats.VPK;

public sealed record VPKFile
{
    public required string Path { get; init; }

    public uint Crc { get; init; }

    public ushort PreloadSize { get; init; }

    public IReadOnlyList<VPKFilePart> Parts { get; init; } = [];

    public IReadOnlyList<byte> PreloadData { get; init; } = [];
}

public sealed record VPKFilePart
{
    public ushort FileNumber { get; init; }

    public uint Offset { get; init; }

    public uint Size { get; init; }
}
