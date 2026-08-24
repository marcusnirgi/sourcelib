namespace SourceLib.Core.Formats.VPK;

public enum VPKVersion : uint
{
    v1 = 1,
    v2 = 2,
}

public sealed record VPKHeader
{
    public const uint SIGNATURE = 0x55aa1234;

    public uint Signature { get; init; }
    public VPKVersion Version { get; init; }
    public uint TreeSize { get; init; }

    public uint? FileDataSectionSize { get; init; }
    public byte[] FileDataSection { get; init; } = [];

    public uint? ArchiveMD5SectionSize { get; init; }
    public byte[] ArchiveMD5Section { get; init; } = [];

    public uint? OtherMD5SectionSize { get; init; }
    public byte[] OtherMD5Section { get; init; } = [];

    public uint? SignatureSectionSize { get; init; }
    public byte[] SignatureSection { get; init; } = [];
}
