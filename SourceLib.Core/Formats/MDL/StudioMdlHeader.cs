using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public enum StudioMdlVersion
{
    V04 = 4,
    V06 = 6,
    V09 = 9,
    V10 = 10,
    V32 = 32,
    V35 = 35,
    V36 = 36,
    V37 = 37,
    V44 = 44,
    V45 = 45,
    V46 = 46,
    V47 = 47,
    V48 = 48,
    V49 = 49,
    V52 = 52,
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

    public static StudioMdlHeader ReadBinary(BinaryReader reader)
    {
        var idChars = reader.ReadChars(4);
        if (new string(idChars) != FormatId)
            throw new InvalidDataException($"Invalid header {new string(idChars)}");

        return new StudioMdlHeader
        {
            Id = idChars,
            Version = (StudioMdlVersion)reader.ReadInt32(),
            Checksum = reader.ReadInt32(),
            Name = new string(reader.ReadChars(NameLength)).TrimEnd('\0'),
            Length = reader.ReadInt32(),
            EyePosition = Vector3.ReadBinary(reader),
            IlluminationCenter = Vector3.ReadBinary(reader),
            HullMin = Vector3.ReadBinary(reader),
            HullMax = Vector3.ReadBinary(reader),
            ViewBbMin = Vector3.ReadBinary(reader),
            ViewBbMax = Vector3.ReadBinary(reader),
            Flags = (StudioMdlFlags)reader.ReadInt32(),
            BoneCount = reader.ReadInt32(),
            BoneIndex = reader.ReadInt32(),
            BoneControllerCount = reader.ReadInt32(),
            BoneControllerIndex = reader.ReadInt32(),
            HitboxSetCount = reader.ReadInt32(),
            HitboxSetIndex = reader.ReadInt32(),
            LocalAnimationCount = reader.ReadInt32(),
            LocalAnimationIndex = reader.ReadInt32(),
            LocalSequencesCount = reader.ReadInt32(),
            LocalSequencesIndex = reader.ReadInt32(),
            ActivityListVersion = reader.ReadInt32(),
            EventsIndexed = reader.ReadInt32(),
            TextureCount = reader.ReadInt32(),
            TextureIndex = reader.ReadInt32(),
            TextureDirectoryCount = reader.ReadInt32(),
            TextureDirectoryIndex = reader.ReadInt32(),
            SkinRefCount = reader.ReadInt32(),
            SkinFamilyCount = reader.ReadInt32(),
            SkinIndex = reader.ReadInt32(),
            BodyPartCount = reader.ReadInt32(),
            BodyPartIndex = reader.ReadInt32(),
            LocalAttachmentCount = reader.ReadInt32(),
            LocalAttachmentIndex = reader.ReadInt32(),
            LocalNodeCount = reader.ReadInt32(),
            LocalNodeIndex = reader.ReadInt32(),
            LocalNodeNameIndex = reader.ReadInt32(),
            FlexDescCount = reader.ReadInt32(),
            FlexDescIndex = reader.ReadInt32(),
            FlexControllerCount = reader.ReadInt32(),
            FlexControllerIndex = reader.ReadInt32(),
            FlexRuleCount = reader.ReadInt32(),
            FlexRuleIndex = reader.ReadInt32(),
            IkChainCount = reader.ReadInt32(),
            IkChainIndex = reader.ReadInt32(),
            MouthCount = reader.ReadInt32(),
            MouthIndex = reader.ReadInt32(),
            LocalPoseParameterCount = reader.ReadInt32(),
            LocalPoseParameterIndex = reader.ReadInt32(),
            SurfacePropIndex = reader.ReadInt32(),
            KeyValueIndex = reader.ReadInt32(),
            KeyValueSize = reader.ReadInt32(),
            LocalIkAutoplayLockCount = reader.ReadInt32(),
            LocalIkAutoplayLockIndex = reader.ReadInt32(),
            Mass = reader.ReadSingle(),
            Contents = reader.ReadInt32(),
            IncludeModelCount = reader.ReadInt32(),
            IncludeModelIndex = reader.ReadInt32(),
            UnusedVirtualModel = reader.ReadInt32(),
            AnimationBlockNameIndex = reader.ReadInt32(),
            AnimationBlockCount = reader.ReadInt32(),
            AnimationBlockIndex = reader.ReadInt32(),
            UnusedAnimationBlockModel = reader.ReadInt32(),
            BoneTableByNameIndex = reader.ReadInt32(),
            UnusedVertexBase = reader.ReadInt32(),
            UnusedIndexBase = reader.ReadInt32(),
            ConstantDirectionalLightDot = reader.ReadByte(),
            RootLod = reader.ReadByte(),
            NumAllowedRootLods = reader.ReadByte(),
            Unused = reader.ReadByte(),
            Unused4 = reader.ReadInt32(),
            FlexControllerUiCount = reader.ReadInt32(),
            FlexControllerUiIndex = reader.ReadInt32(),
            VertAnimFixedPointScale = reader.ReadSingle(),
            Unused3 = reader.ReadInt32(),
            StudioHdr2Index = reader.ReadInt32(),
            Unused2 = reader.ReadInt32(),
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Id);
        writer.Write((int)Version);
        writer.Write(Checksum);
        var nameBuf = (Name + '\0').PadRight(NameLength, '\0')[..NameLength];
        writer.Write(nameBuf.ToCharArray());
        writer.Write(Length);
        writer.Write(EyePosition.X);
        writer.Write(EyePosition.Y);
        writer.Write(EyePosition.Z);
        writer.Write(IlluminationCenter.X);
        writer.Write(IlluminationCenter.Y);
        writer.Write(IlluminationCenter.Z);
        writer.Write(HullMin.X);
        writer.Write(HullMin.Y);
        writer.Write(HullMin.Z);
        writer.Write(HullMax.X);
        writer.Write(HullMax.Y);
        writer.Write(HullMax.Z);
        writer.Write(ViewBbMin.X);
        writer.Write(ViewBbMin.Y);
        writer.Write(ViewBbMin.Z);
        writer.Write(ViewBbMax.X);
        writer.Write(ViewBbMax.Y);
        writer.Write(ViewBbMax.Z);
        writer.Write((int)Flags);
        writer.Write(BoneCount);
        writer.Write(BoneIndex);
        writer.Write(BoneControllerCount);
        writer.Write(BoneControllerIndex);
        writer.Write(HitboxSetCount);
        writer.Write(HitboxSetIndex);
        writer.Write(LocalAnimationCount);
        writer.Write(LocalAnimationIndex);
        writer.Write(LocalSequencesCount);
        writer.Write(LocalSequencesIndex);
        writer.Write(ActivityListVersion);
        writer.Write(EventsIndexed);
        writer.Write(TextureCount);
        writer.Write(TextureIndex);
        writer.Write(TextureDirectoryCount);
        writer.Write(TextureDirectoryIndex);
        writer.Write(SkinRefCount);
        writer.Write(SkinFamilyCount);
        writer.Write(SkinIndex);
        writer.Write(BodyPartCount);
        writer.Write(BodyPartIndex);
        writer.Write(LocalAttachmentCount);
        writer.Write(LocalAttachmentIndex);
        writer.Write(LocalNodeCount);
        writer.Write(LocalNodeIndex);
        writer.Write(LocalNodeNameIndex);
        writer.Write(FlexDescCount);
        writer.Write(FlexDescIndex);
        writer.Write(FlexControllerCount);
        writer.Write(FlexControllerIndex);
        writer.Write(FlexRuleCount);
        writer.Write(FlexRuleIndex);
        writer.Write(IkChainCount);
        writer.Write(IkChainIndex);
        writer.Write(MouthCount);
        writer.Write(MouthIndex);
        writer.Write(LocalPoseParameterCount);
        writer.Write(LocalPoseParameterIndex);
        writer.Write(SurfacePropIndex);
        writer.Write(KeyValueIndex);
        writer.Write(KeyValueSize);
        writer.Write(LocalIkAutoplayLockCount);
        writer.Write(LocalIkAutoplayLockIndex);
        writer.Write(Mass);
        writer.Write(Contents);
        writer.Write(IncludeModelCount);
        writer.Write(IncludeModelIndex);
        writer.Write(UnusedVirtualModel);
        writer.Write(AnimationBlockNameIndex);
        writer.Write(AnimationBlockCount);
        writer.Write(AnimationBlockIndex);
        writer.Write(UnusedAnimationBlockModel);
        writer.Write(BoneTableByNameIndex);
        writer.Write(UnusedVertexBase);
        writer.Write(UnusedIndexBase);
        writer.Write(ConstantDirectionalLightDot);
        writer.Write(RootLod);
        writer.Write(NumAllowedRootLods);
        writer.Write(Unused);
        writer.Write(Unused4);
        writer.Write(FlexControllerUiCount);
        writer.Write(FlexControllerUiIndex);
        writer.Write(VertAnimFixedPointScale);
        writer.Write(Unused3);
        writer.Write(StudioHdr2Index);
        writer.Write(Unused2);
    }
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

    public static StudioMdlHeader2 ReadBinary(BinaryReader reader) =>
        new()
        {
            SourceBoneTransformCount = reader.ReadInt32(),
            SourceBoneTransformIndex = reader.ReadInt32(),
            IlluminationPositionAttachmentIndex = reader.ReadInt32(),
            MaxEyeDeflection = reader.ReadSingle(),
            LinearBoneIndex = reader.ReadInt32(),
            NameIndex = reader.ReadInt32(),
            BoneFlexDriverCount = reader.ReadInt32(),
            BoneFlexDriverIndex = reader.ReadInt32(),
            Reserved = Enumerable.Range(0, ReservedCount).Select(_ => reader.ReadInt32()).ToArray(),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(SourceBoneTransformCount);
        writer.Write(SourceBoneTransformIndex);
        writer.Write(IlluminationPositionAttachmentIndex);
        writer.Write(MaxEyeDeflection);
        writer.Write(LinearBoneIndex);
        writer.Write(NameIndex);
        writer.Write(BoneFlexDriverCount);
        writer.Write(BoneFlexDriverIndex);
        foreach (var r in Reserved)
            writer.Write(r);
    }
}
