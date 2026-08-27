using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlAnimDesc
{
    public required int BasePtr { get; set; }
    public required int NameIndex { get; set; }
    public required float Fps { get; set; }
    public required int Flags { get; set; }
    public required int NumFrames { get; set; }
    public required int NumMovements { get; set; }
    public required int MovementIndex { get; set; }
    public required int[] Unused1 { get; set; }
    public required int AnimBlock { get; set; }
    public required int AnimIndex { get; set; }
    public required int NumIkRules { get; set; }
    public required int IkRuleIndex { get; set; }
    public required int AnimBlockIkRuleIndex { get; set; }
    public required int NumLocalHierarchy { get; set; }
    public required int LocalHierarchyIndex { get; set; }
    public required int SectionIndex { get; set; }
    public required int SectionFrames { get; set; }
    public required short ZeroFrameSpan { get; set; }
    public required short ZeroFrameCount { get; set; }
    public required int ZeroFrameIndex { get; set; }
    public required float ZeroFrameStallTime { get; set; }
    public required int Offset { get; set; }

    public required string Name { get; set; }
    public required IList<StudioMdlMovement> Movements { get; set; }
    public required IList<StudioMdlAnimSection> Sections { get; set; }
    public required IList<StudioMdlIkRule> IkRules { get; set; }
    public required IList<StudioMdlLocalHierarchy> LocalHierarchy { get; set; }
}

public sealed class StudioMdlAnimSection
{
    public required int AnimBlock { get; set; }
    public required int AnimIndex { get; set; }
}

public sealed class StudioMdlMovement
{
    public required int EndFrame { get; set; }
    public required int MotionFlags { get; set; }
    public required float V0 { get; set; }
    public required float V1 { get; set; }
    public required float Angle { get; set; }
    public required Vector3 Vector { get; set; }
    public required Vector3 Position { get; set; }
}

public sealed class StudioMdlAnimValuePtr
{
    public required short[] Offset { get; set; }
}

public sealed class StudioMdlAnim
{
    public required byte Bone { get; set; }
    public required byte Flags { get; set; }
    public required short NextOffset { get; set; }
}