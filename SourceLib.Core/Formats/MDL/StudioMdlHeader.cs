using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public enum StudioMdlVersion
{
    V04,
    V06,
    V09,
    V10,
    V32,
    V35,
    V36,
    V37,
    V44,
    V46,
    V48,
    V49,
    V52,
}

public sealed class StudioMdlHeader
{
    public static string FormatId = "IDST";

    public static int NameLength = 64;

    public required char[] Id { get; set; }

    public required StudioMdlVersion Version { get; set; }

    public required int Checksum { get; set; }

    public required string Name { get; set; }

    public required int Length { get; set; }

    public required Vector3 EyePosition { get; set; }

    public required Vector3 IlluminationCenter { get; set; }

    public required Vector3 HullMin { get; set; }

    public required Vector3 HullMax { get; set; }

    public required Vector3 ViewBbMin { get; set; }

    public required Vector3 ViewBbMax { get; set; }

    public required StudioMdlFlags Flags { get; set; }

    public required int BoneCount { get; set; }

    public required int BoneIndex { get; set; }

    public required int BoneControllerCount { get; set; }

    public required int BoneControllerIndex { get; set; }

    public required int HitboxSetCount { get; set; }

    public required int HitboxSetIndex { get; set; }

    public required int LocalAnimationCount { get; set; }

    public required int LocalAnimationIndex { get; set; }

    public required int LocalSequencesCount { get; set; }

    public required int LocalSequencesIndex { get; set; }

    public required int ActivityListVersion { get; set; }

    public required int EventsIndexed { get; set; }

    public required int TextureCount { get; set; }

    public required int TextureIndex { get; set; }

    public required int TextureDirectoryCount { get; set; }

    public required int TextureDirectoryIndex { get; set; }

    public required int SkinRefCount { get; set; }

    public required int SkinFamilyCount { get; set; }

    public required int SkinIndex { get; set; }

    public required int BodyPartCount { get; set; }

    public required int BodyPartIndex { get; set; }

    public required int LocalAttachmentCount { get; set; }

    public required int LocalAttachmentIndex { get; set; }

    public required int LocalNodeCount { get; set; }

    public required int LocalNodeIndex { get; set; }

    public required int LocalNodeNameIndex { get; set; }

    public required int FlexDescCount { get; set; }

    public required int FlexDescIndex { get; set; }

    public required int FlexControllerCount { get; set; }

    public required int FlexControllerIndex { get; set; }

    public required int FlexRuleCount { get; set; }

    public required int FlexRuleIndex { get; set; }

    public required int IkChainCount { get; set; }

    public required int IkChainIndex { get; set; }

    public required int MouthCount { get; set; }

    public required int MouthIndex { get; set; }

    public required int LocalPoseParameterCount { get; set; }

    public required int LocalPoseParameterIndex { get; set; }

    public required int SurfacePropIndex { get; set; }

    public required int KeyValueIndex { get; set; }

    public required int KeyValueSize { get; set; }

    public required int LocalIkAutoplayLockCount { get; set; }

    public required int LocalIkAutoplayLockIndex { get; set; }

    public required float Mass { get; set; }

    public required int Contents { get; set; }

    public required int IncludeModelCount { get; set; }

    public required int IncludeModelIndex { get; set; }

    public required int UnusedVirtualModel { get; set; }

    public required int AnimationBlockNameIndex { get; set; }

    public required int AnimationBlockCount { get; set; }

    public required int AnimationBlockIndex { get; set; }

    public required int UnusedAnimationBlockModel { get; set; }

    public required int BoneTableByNameIndex { get; set; }

    public required int UnusedVertexBase { get; set; }

    public required int UnusedIndexBase { get; set; }

    public required byte ConstantDirectionalLightDot { get; set; }

    public required byte RootLod { get; set; }

    public required byte NumAllowedRootLods { get; set; }

    public required byte Unused { get; set; }

    public required int Unused4 { get; set; }

    public required int FlexControllerUiCount { get; set; }

    public required int FlexControllerUiIndex { get; set; }

    public required float VertAnimFixedPointScale { get; set; }

    public required int Unused3 { get; set; }

    public required int StudioHdr2Index { get; set; }

    public required int Unused2 { get; set; }
}

public sealed class StudioMdlHeader2
{
    public static readonly int ReservedCount = 56;
    public required int SourceBoneTransformCount { get; set; }

    public required int SourceBoneTransformIndex { get; set; }

    public required int IlluminationPositionAttachmentIndex { get; set; }

    public required float MaxEyeDeflection { get; set; }

    public required int LinearBoneIndex { get; set; }

    public required int NameIndex { get; set; }

    public required int BoneFlexDriverCount { get; set; }

    public required int BoneFlexDriverIndex { get; set; }

    public required int[] Reserved { get; set; }
}
