using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlBone
{
    public required int NameIndex { get; set; }
    public required int Parent { get; set; }
    public required int[] BoneController { get; set; }
    public required Vector3 Position { get; set; }
    public required Quaternion Quaternion { get; set; }
    public required Vector3 Rotation { get; set; }
    public required Vector3 PositionScale { get; set; }
    public required Vector3 RotationScale { get; set; }
    public required Matrix PoseToBone { get; set; }
    public required Quaternion AlignmentQuaternion { get; set; }
    public required StudioMdlBoneFlags Flags { get; set; }
    public required int ProcedureType { get; set; }
    public required int ProcedureIndex { get; set; }
    public required int PhysicsBone { get; set; }
    public required int SurfacePropIndex { get; set; }
    public required int Contents { get; set; }
    public required int[] Unused { get; set; }

    public required string Name { get; set; }
    public required string SurfaceProp { get; set; }
    public IStudioMdlBoneProcedure? Procedural { get; set; }
}

public sealed class StudioMdlLinearBone
{
    public required int BoneCount { get; set; }
    public required int FlagsIndex { get; set; }
    public required int ParentIndex { get; set; }
    public required int PositionIndex { get; set; }
    public required int QuaternionIndex { get; set; }
    public required int RotationIndex { get; set; }
    public required int PoseToBoneIndex { get; set; }
    public required int PositionScaleIndex { get; set; }
    public required int RotationScaleIndex { get; set; }
    public required int AlignmentQuaternionIndex { get; set; }
    public required int[] Unused { get; set; }
}

public sealed class StudioMdlSourceBoneTransform
{
    public required int NameIndex { get; set; }
    public required Matrix PreTransform { get; set; }
    public required Matrix PostTransform { get; set; }
}

public sealed class StudioMdlBoneFlexDriverControl
{
    public required int BoneComponent { get; set; }
    public required int FlexControllerIndex { get; set; }
    public required float Min { get; set; }
    public required float Max { get; set; }
}

public sealed class StudioMdlBoneFlexDriver
{
    public required int BoneIndex { get; set; }
    public required int ControlCount { get; set; }
    public required int ControlIndex { get; set; }
    public required int[] Unused { get; set; }
}

public sealed class StudioMdlBoneController
{
    public required int Bone { get; set; }
    public required int Type { get; set; }
    public required float Start { get; set; }
    public required float End { get; set; }
    public required int Rest { get; set; }
    public required int InputField { get; set; }
    public required int[] Unused { get; set; }
}
