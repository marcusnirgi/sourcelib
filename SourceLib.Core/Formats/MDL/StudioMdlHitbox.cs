using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlHitboxSet
{
    public required int NameIndex { get; set; }

    public required int HitboxCount { get; set; }

    public required int HitboxIndex { get; set; }

    public required string Name { get; set; }

    public required IList<StudioMdlHitbox> Hitboxes { get; set; }
}

public sealed class StudioMdlHitbox
{
    public required int Bone { get; set; }

    public required int Group { get; set; }

    public required Vector3 Min { get; set; }

    public required Vector3 Max { get; set; }

    public required int NameIndex { get; set; }

    public required int[] Unused { get; set; }

    public required string Name { get; set; }
}
