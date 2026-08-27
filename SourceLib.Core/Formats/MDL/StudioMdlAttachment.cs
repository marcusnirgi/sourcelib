using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlAttachment
{
    public required int NameIndex { get; set; }

    public required uint Flags { get; set; }

    public required int LocalBone { get; set; }

    public required Matrix Local { get; set; }

    public required int[] Unused { get; set; }

    public required string Name { get; set; }
}
