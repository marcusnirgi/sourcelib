namespace SourceLib.Core.Formats.VPK;

public sealed class VPK
{
    public IReadOnlyList<VPKFile> Files { get; init; } = [];
}
