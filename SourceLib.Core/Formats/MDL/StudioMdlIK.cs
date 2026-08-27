using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlIkLink
{
    public required int Bone { get; set; }
    public required Vector3 KneeDir { get; set; }
    public required Vector3 Unused0 { get; set; }
}

public sealed class StudioMdlIkChain
{
    public required int NameIndex { get; set; }
    public required int LinkType { get; set; }
    public required int NumLinks { get; set; }
    public required int LinkIndex { get; set; }
    public required IList<StudioMdlIkLink> Links { get; set; }
    public required string Name { get; set; }
}

public sealed class StudioMdlIkError
{
    public required Vector3 Position { get; set; }
    public required Quaternion Quaternion { get; set; }
}

public sealed class StudioMdlCompressedIkError
{
    public required float[] Scale { get; set; }
    public required short[] Offset { get; set; }
}

public sealed class StudioMdlIkRule
{
    public required int Index { get; set; }
    public required int Type { get; set; }
    public required int Chain { get; set; }
    public required int Bone { get; set; }
    public required int Slot { get; set; }
    public required float Height { get; set; }
    public required float Radius { get; set; }
    public required float Floor { get; set; }
    public required Vector3 Position { get; set; }
    public required Quaternion Quaternion { get; set; }
    public required int CompressedIkErrorIndex { get; set; }
    public required int Unused2 { get; set; }
    public required int IStart { get; set; }
    public required int IkErrorIndex { get; set; }
    public required float Start { get; set; }
    public required float Peak { get; set; }
    public required float Tail { get; set; }
    public required float End { get; set; }
    public required float Unused3 { get; set; }
    public required float Contact { get; set; }
    public required float Drop { get; set; }
    public required float Top { get; set; }
    public required int Unused6 { get; set; }
    public required int Unused7 { get; set; }
    public required int Unused8 { get; set; }
    public required int AttachmentIndex { get; set; }
    public required int[] Unused { get; set; }
    public required string Attachment { get; set; }
}

public sealed class StudioMdlLocalHierarchy
{
    public required int Bone { get; set; }
    public required int NewParent { get; set; }
    public required float Start { get; set; }
    public required float Peak { get; set; }
    public required float Tail { get; set; }
    public required float End { get; set; }
    public required int IStart { get; set; }
    public required int LocalAnimIndex { get; set; }
    public required int[] Unused { get; set; }
}