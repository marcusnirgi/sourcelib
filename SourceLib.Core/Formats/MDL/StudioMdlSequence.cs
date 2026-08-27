using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlSeqDesc
{
    public required int BasePtr { get; set; }
    public required int LabelIndex { get; set; }
    public required int ActivityNameIndex { get; set; }
    public required int Flags { get; set; }
    public required int Activity { get; set; }
    public required int ActWeight { get; set; }
    public required int NumEvents { get; set; }
    public required int EventIndex { get; set; }
    public required Vector3 BbMin { get; set; }
    public required Vector3 BbMax { get; set; }
    public required int NumBlends { get; set; }
    public required int AnimIndexIndex { get; set; }
    public required int MovementIndex { get; set; }
    public required int[] GroupSize { get; set; }
    public required int[] ParamIndex { get; set; }
    public required float[] ParamStart { get; set; }
    public required float[] ParamEnd { get; set; }
    public required int ParamParent { get; set; }
    public required float FadeInTime { get; set; }
    public required float FadeOutTime { get; set; }
    public required int LocalEntryNode { get; set; }
    public required int LocalExitNode { get; set; }
    public required int NodeFlags { get; set; }
    public required float EntryPhase { get; set; }
    public required float ExitPhase { get; set; }
    public required float LastFrame { get; set; }
    public required int NextSeq { get; set; }
    public required int Pose { get; set; }
    public required int NumIkRules { get; set; }
    public required int NumAutoLayers { get; set; }
    public required int AutoLayerIndex { get; set; }
    public required int WeightListIndex { get; set; }
    public required int PoseKeyIndex { get; set; }
    public required int NumIkLocks { get; set; }
    public required int IkLockIndex { get; set; }
    public required int KeyValueIndex { get; set; }
    public required int KeyValueSize { get; set; }
    public required int CyclePoseIndex { get; set; }
    public required int ActivityModifierIndex { get; set; }
    public required int NumActivityModifiers { get; set; }
    public required int[] Unused { get; set; }
    public required int Offset { get; set; }

    public required string Label { get; set; }
    public required string ActivityName { get; set; }
    public required IList<StudioMdlEvent> Events { get; set; }
    public required IList<StudioMdlAutoLayer> AutoLayers { get; set; }
    public required IList<StudioMdlIkLock> IkLocks { get; set; }
    public required IList<IList<int>> AnimIndices { get; set; }
}

public sealed class StudioMdlEvent
{
    public required float Cycle { get; set; }
    public required int Event { get; set; }
    public required int Type { get; set; }
    public required string Options { get; set; }
    public required int EventIndex { get; set; }
    public required string Name { get; set; }
}

public sealed class StudioMdlAutoLayer
{
    public required short Sequence { get; set; }
    public required short Pose { get; set; }
    public required int Flags { get; set; }
    public required float Start { get; set; }
    public required float Peak { get; set; }
    public required float Tail { get; set; }
    public required float End { get; set; }
}

public sealed class StudioMdlIkLock
{
    public required int Chain { get; set; }
    public required float PosWeight { get; set; }
    public required float LocalQWeight { get; set; }
    public required int Flags { get; set; }
    public required int[] Unused { get; set; }
}