using System.Buffers;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdl
{
    public StudioMdlHeader Header { get; set; }
    public StudioMdlHeader2? Header2 { get; set; }
    public IList<StudioMdlBone> Bones { get; set; }
    public IList<StudioMdlTexture> Textures { get; set; }
    public IList<string> TextureDirectories { get; set; }
    public IList<int[]> Skins { get; set; }
    public IList<StudioMdlBodyPart> BodyParts { get; set; }
    public IList<StudioMdlHitboxSet> HitboxSets { get; set; }
    public IList<StudioMdlAttachment> Attachments { get; set; }
    public IList<StudioMdlBoneController> BoneControllers { get; set; }
    public IList<StudioMdlFlexDesc> FlexDescs { get; set; }
    public IList<StudioMdlFlexController> FlexControllers { get; set; }
    public IList<StudioMdlFlexRule> FlexRules { get; set; }
    public IList<StudioMdlFlexControllerUi> FlexControllerUi { get; set; }
    public IList<StudioMdlIkChain> IkChains { get; set; }
    public IList<StudioMdlPoseParamDesc> PoseParameters { get; set; }
    public IList<StudioMdlMouth> Mouths { get; set; }
    public IList<StudioMdlModelGroup> IncludeModels { get; set; }
    public IList<StudioMdlAnimBlock> AnimBlocks { get; set; }
    public IList<StudioMdlAnimDesc> Animations { get; set; }
    public IList<StudioMdlSeqDesc> Sequences { get; set; }
    public StudioMdlLinearBone? LinearBones { get; set; }
    public IList<StudioMdlSourceBoneTransform> SourceBoneTransforms { get; set; }
    public IList<StudioMdlBoneFlexDriver> BoneFlexDrivers { get; set; }
    public string SurfaceProp { get; set; }
    public string KeyValues { get; set; }
    public byte[] BoneTableByName { get; set; }
    public IList<int>? TextureDirectoryOffsets { get; set; }

    public StudioMdl(
        StudioMdlHeader header,
        StudioMdlHeader2? header2,
        IList<StudioMdlBone> bones,
        IList<StudioMdlTexture> textures,
        IList<string> textureDirectories,
        IList<int[]> skins,
        IList<StudioMdlBodyPart> bodyParts,
        IList<StudioMdlHitboxSet> hitboxSets,
        IList<StudioMdlAttachment> attachments,
        IList<StudioMdlBoneController> boneControllers,
        IList<StudioMdlFlexDesc> flexDescs,
        IList<StudioMdlFlexController> flexControllers,
        IList<StudioMdlFlexRule> flexRules,
        IList<StudioMdlFlexControllerUi> flexControllerUi,
        IList<StudioMdlIkChain> ikChains,
        IList<StudioMdlPoseParamDesc> poseParameters,
        IList<StudioMdlMouth> mouths,
        IList<StudioMdlModelGroup> includeModels,
        IList<StudioMdlAnimBlock> animBlocks,
        IList<StudioMdlAnimDesc> animations,
        IList<StudioMdlSeqDesc> sequences,
        StudioMdlLinearBone? linearBones,
        IList<StudioMdlSourceBoneTransform> sourceBoneTransforms,
        IList<StudioMdlBoneFlexDriver> boneFlexDrivers,
        string surfaceProp,
        string keyValues,
        byte[] boneTableByName,
        IList<int>? textureDirectoryOffsets = null
    )
    {
        Header = header;
        Header2 = header2;
        Bones = bones;
        Textures = textures;
        TextureDirectories = textureDirectories;
        Skins = skins;
        BodyParts = bodyParts;
        HitboxSets = hitboxSets;
        Attachments = attachments;
        BoneControllers = boneControllers;
        FlexDescs = flexDescs;
        FlexControllers = flexControllers;
        FlexRules = flexRules;
        FlexControllerUi = flexControllerUi;
        IkChains = ikChains;
        PoseParameters = poseParameters;
        Mouths = mouths;
        IncludeModels = includeModels;
        AnimBlocks = animBlocks;
        Animations = animations;
        Sequences = sequences;
        LinearBones = linearBones;
        SourceBoneTransforms = sourceBoneTransforms;
        BoneFlexDrivers = boneFlexDrivers;
        SurfaceProp = surfaceProp;
        KeyValues = keyValues;
        BoneTableByName = boneTableByName;
        TextureDirectoryOffsets = textureDirectoryOffsets;
    }

    public StudioMdlTexture GetTexture(int skinIndex, int materialIndex)
    {
        if (skinIndex < 0 || skinIndex >= Skins.Count)
            throw new IndexOutOfRangeException($"Skin index {skinIndex} is out of range.");

        if (materialIndex < 0 || materialIndex >= Skins[skinIndex].Length)
            throw new IndexOutOfRangeException($"Material index {materialIndex} is out of range.");

        var textureIndex = Skins[skinIndex][materialIndex];

        if (textureIndex < 0 || textureIndex >= Textures.Count)
            throw new IndexOutOfRangeException($"Texture index {textureIndex} is out of range.");

        return Textures[textureIndex];
    }

    public static StudioMdl ReadBinary(BinaryReader reader)
    {
        var header = StudioMdlHeader.ReadBinary(reader);

        StudioMdlHeader2? header2 = null;

        if (header.StudioHdr2Index != 0)
        {
            reader.BaseStream.Position = header.StudioHdr2Index;
            header2 = StudioMdlHeader2.ReadBinary(reader);
        }

        reader.BaseStream.Position = header.BoneIndex;
        var bones = Enumerable
            .Range(0, header.BoneCount)
            .Select(_ => StudioMdlBone.ReadBinary(reader))
            .ToList();

        reader.BaseStream.Position = header.TextureIndex;
        var textures = Enumerable
            .Range(0, header.TextureCount)
            .Select(_ => StudioMdlTexture.ReadBinary(reader))
            .ToList();

        var (textureDirectories, textureDirectoryOffsets) = ParseTextureDirectories(reader, header);
        var skins = ParseSkins(reader, header);

        reader.BaseStream.Position = header.BodyPartIndex;
        var bodyParts = Enumerable
            .Range(0, header.BodyPartCount)
            .Select(_ => StudioMdlBodyPart.ReadBinary(reader))
            .ToList();

        reader.BaseStream.Position = header.HitboxSetIndex;
        var hitboxSets = Enumerable
            .Range(0, header.HitboxSetCount)
            .Select(_ => StudioMdlHitboxSet.ReadBinary(reader))
            .ToList();

        reader.BaseStream.Position = header.LocalAttachmentIndex;
        var attachments = Enumerable
            .Range(0, header.LocalAttachmentCount)
            .Select(_ => StudioMdlAttachment.ReadBinary(reader))
            .ToList();

        var boneControllers =
            header.BoneControllerCount > 0 && header.BoneControllerIndex != 0
                ? ReadArray(
                    reader,
                    header.BoneControllerIndex,
                    header.BoneControllerCount,
                    StudioMdlBoneController.ReadBinary
                )
                : [];

        var flexDescs =
            header.FlexDescCount > 0 && header.FlexDescIndex != 0
                ? ReadArray(
                    reader,
                    header.FlexDescIndex,
                    header.FlexDescCount,
                    StudioMdlFlexDesc.ReadBinary
                )
                : [];

        var flexControllers =
            header.FlexControllerCount > 0 && header.FlexControllerIndex != 0
                ? ReadArray(
                    reader,
                    header.FlexControllerIndex,
                    header.FlexControllerCount,
                    StudioMdlFlexController.ReadBinary
                )
                : [];

        var flexRules =
            header.FlexRuleCount > 0 && header.FlexRuleIndex != 0
                ? ReadArray(
                    reader,
                    header.FlexRuleIndex,
                    header.FlexRuleCount,
                    StudioMdlFlexRule.ReadBinary
                )
                : [];

        var flexControllerUi =
            header.FlexControllerUiCount > 0 && header.FlexControllerUiIndex != 0
                ? ReadArray(
                    reader,
                    header.FlexControllerUiIndex,
                    header.FlexControllerUiCount,
                    StudioMdlFlexControllerUi.ReadBinary
                )
                : [];

        var ikChains =
            header.IkChainCount > 0 && header.IkChainIndex != 0
                ? ReadArray(
                    reader,
                    header.IkChainIndex,
                    header.IkChainCount,
                    StudioMdlIkChain.ReadBinary
                )
                : [];

        var poseParameters =
            header.LocalPoseParameterCount > 0 && header.LocalPoseParameterIndex != 0
                ? ReadArray(
                    reader,
                    header.LocalPoseParameterIndex,
                    header.LocalPoseParameterCount,
                    StudioMdlPoseParamDesc.ReadBinary
                )
                : [];

        var mouths =
            header.MouthCount > 0 && header.MouthIndex != 0
                ? ReadArray(reader, header.MouthIndex, header.MouthCount, StudioMdlMouth.ReadBinary)
                : [];

        var includeModels =
            header.IncludeModelCount > 0 && header.IncludeModelIndex != 0
                ? ReadArray(
                    reader,
                    header.IncludeModelIndex,
                    header.IncludeModelCount,
                    StudioMdlModelGroup.ReadBinary
                )
                : [];

        var animBlocks =
            header.AnimationBlockCount > 0 && header.AnimationBlockIndex != 0
                ? ReadArray(
                    reader,
                    header.AnimationBlockIndex,
                    header.AnimationBlockCount,
                    StudioMdlAnimBlock.ReadBinary
                )
                : [];

        var animations = new List<StudioMdlAnimDesc>();
        if (header.LocalAnimationCount > 0 && header.LocalAnimationIndex != 0)
        {
            reader.BaseStream.Position = header.LocalAnimationIndex;
            for (var i = 0; i < header.LocalAnimationCount; i++)
            {
                animations.Add(StudioMdlAnimDesc.ReadBinary(reader));
            }

            for (var i = 0; i < animations.Count; i++)
            {
                if (animations[i].AnimIndex != 0)
                {
                    var endOffset = 0;
                    if (i + 1 < animations.Count && animations[i + 1].AnimIndex != 0)
                    {
                        endOffset = animations[i + 1].Offset + animations[i + 1].AnimIndex;
                    }
                    else if (header.LocalSequencesIndex != 0)
                    {
                        endOffset = header.LocalSequencesIndex;
                    }

                    if (endOffset > 0)
                    {
                        reader.BaseStream.Position = animations[i].Offset + animations[i].AnimIndex;
                        animations[i].Nodes = StudioMdlAnimationNode.ReadChain(reader, endOffset);
                    }
                }
            }
        }

        var sequences =
            header.LocalSequencesCount > 0 && header.LocalSequencesIndex != 0
                ? ReadArray(
                    reader,
                    header.LocalSequencesIndex,
                    header.LocalSequencesCount,
                    r => StudioMdlSeqDesc.ReadBinary(r, header.BoneCount)
                )
                : [];

        StudioMdlLinearBone? linearBones = null;

        if (header2 is not null && header2.LinearBoneIndex != 0)
        {
            reader.BaseStream.Position = header.StudioHdr2Index + header2.LinearBoneIndex;

            linearBones = StudioMdlLinearBone.ReadBinary(reader);
        }

        var sourceBoneTransforms = new List<StudioMdlSourceBoneTransform>();

        if (header2 is not null && header2.SourceBoneTransformCount > 0)
        {
            reader.BaseStream.Position = header2.SourceBoneTransformIndex;

            sourceBoneTransforms = Enumerable
                .Range(0, header2.SourceBoneTransformCount)
                .Select(_ => StudioMdlSourceBoneTransform.ReadBinary(reader))
                .ToList();
        }

        var boneFlexDrivers = new List<StudioMdlBoneFlexDriver>();

        if (header2 is not null && header2.BoneFlexDriverCount > 0)
        {
            reader.BaseStream.Position = header.StudioHdr2Index + header2.BoneFlexDriverIndex;

            boneFlexDrivers = Enumerable
                .Range(0, header2.BoneFlexDriverCount)
                .Select(_ => StudioMdlBoneFlexDriver.ReadBinary(reader))
                .ToList();
        }

        var surfaceProp =
            header.SurfacePropIndex > 0
                ? BinaryReading.ReadStringUntilAt(reader, header.SurfacePropIndex, 0)
                : string.Empty;

        var keyValues = ReadKeyValues(reader, header.KeyValueIndex, header.KeyValueSize);

        var boneTableByName = ReadBoneTableByName(
            reader,
            header.BoneTableByNameIndex,
            header.BoneCount
        );

        return new StudioMdl(
            header,
            header2,
            bones,
            textures,
            textureDirectories,
            skins,
            bodyParts,
            hitboxSets,
            attachments,
            boneControllers,
            flexDescs,
            flexControllers,
            flexRules,
            flexControllerUi,
            ikChains,
            poseParameters,
            mouths,
            includeModels,
            animBlocks,
            animations,
            sequences,
            linearBones,
            sourceBoneTransforms,
            boneFlexDrivers,
            surfaceProp,
            keyValues,
            boneTableByName,
            textureDirectoryOffsets
        );
    }

    public void WriteBinary(BinaryWriter writer)
    {
        Header.WriteBinary(writer);

        if (Header2 is not null && Header.StudioHdr2Index != 0)
        {
            writer.BaseStream.Position = Header.StudioHdr2Index;
            Header2.WriteBinary(writer);
        }

        if (Header.BoneCount > 0 && Header.BoneIndex != 0)
        {
            writer.BaseStream.Position = Header.BoneIndex;
            foreach (var bone in Bones)
                bone.WriteBinary(writer);
        }

        if (Header.TextureCount > 0 && Header.TextureIndex != 0)
        {
            writer.BaseStream.Position = Header.TextureIndex;
            foreach (var texture in Textures)
                texture.WriteBinary(writer);
        }

        WriteTextureDirectories(writer, Header, TextureDirectories, TextureDirectoryOffsets);

        WriteSkins(writer, Header, Skins);

        if (Header.BodyPartCount > 0 && Header.BodyPartIndex != 0)
        {
            writer.BaseStream.Position = Header.BodyPartIndex;
            foreach (var bodyPart in BodyParts)
                bodyPart.WriteBinary(writer);
        }

        if (Header.HitboxSetCount > 0 && Header.HitboxSetIndex != 0)
        {
            writer.BaseStream.Position = Header.HitboxSetIndex;
            foreach (var hitboxSet in HitboxSets)
                hitboxSet.WriteBinary(writer);
        }

        if (Header.LocalAttachmentCount > 0 && Header.LocalAttachmentIndex != 0)
        {
            writer.BaseStream.Position = Header.LocalAttachmentIndex;
            foreach (var attachment in Attachments)
                attachment.WriteBinary(writer);
        }

        if (Header.BoneControllerCount > 0 && Header.BoneControllerIndex != 0)
        {
            writer.BaseStream.Position = Header.BoneControllerIndex;
            foreach (var bc in BoneControllers)
                bc.WriteBinary(writer);
        }

        if (Header.FlexDescCount > 0 && Header.FlexDescIndex != 0)
        {
            writer.BaseStream.Position = Header.FlexDescIndex;
            foreach (var fd in FlexDescs)
                fd.WriteBinary(writer);
        }

        if (Header.FlexControllerCount > 0 && Header.FlexControllerIndex != 0)
        {
            writer.BaseStream.Position = Header.FlexControllerIndex;
            foreach (var fc in FlexControllers)
                fc.WriteBinary(writer);
        }

        if (Header.FlexRuleCount > 0 && Header.FlexRuleIndex != 0)
        {
            writer.BaseStream.Position = Header.FlexRuleIndex;
            foreach (var fr in FlexRules)
                fr.WriteBinary(writer);
        }

        if (Header.FlexControllerUiCount > 0 && Header.FlexControllerUiIndex != 0)
        {
            writer.BaseStream.Position = Header.FlexControllerUiIndex;
            foreach (var fcUi in FlexControllerUi)
                fcUi.WriteBinary(writer);
        }

        if (Header.IkChainCount > 0 && Header.IkChainIndex != 0)
        {
            writer.BaseStream.Position = Header.IkChainIndex;
            foreach (var ik in IkChains)
                ik.WriteBinary(writer);
        }

        if (Header.LocalPoseParameterCount > 0 && Header.LocalPoseParameterIndex != 0)
        {
            writer.BaseStream.Position = Header.LocalPoseParameterIndex;
            foreach (var pp in PoseParameters)
                pp.WriteBinary(writer);
        }

        if (Header.MouthCount > 0 && Header.MouthIndex != 0)
        {
            writer.BaseStream.Position = Header.MouthIndex;
            foreach (var mouth in Mouths)
                mouth.WriteBinary(writer);
        }

        if (Header.IncludeModelCount > 0 && Header.IncludeModelIndex != 0)
        {
            writer.BaseStream.Position = Header.IncludeModelIndex;
            foreach (var modelGroup in IncludeModels)
                modelGroup.WriteBinary(writer);
        }

        if (Header.AnimationBlockCount > 0 && Header.AnimationBlockIndex != 0)
        {
            writer.BaseStream.Position = Header.AnimationBlockIndex;
            foreach (var animBlock in AnimBlocks)
                animBlock.WriteBinary(writer);
        }

        if (Header.LocalAnimationCount > 0 && Header.LocalAnimationIndex != 0)
        {
            writer.BaseStream.Position = Header.LocalAnimationIndex;
            foreach (var anim in Animations)
                anim.WriteBinary(writer);
        }

        if (Header.LocalSequencesCount > 0 && Header.LocalSequencesIndex != 0)
        {
            writer.BaseStream.Position = Header.LocalSequencesIndex;
            foreach (var seq in Sequences)
                seq.WriteBinary(writer);
        }

        if (Header2 is not null && Header2.LinearBoneIndex != 0 && LinearBones is not null)
        {
            writer.BaseStream.Position = Header.StudioHdr2Index + Header2.LinearBoneIndex;
            LinearBones.WriteBinary(writer);
        }

        if (
            Header2 is not null
            && Header2.SourceBoneTransformCount > 0
            && Header2.SourceBoneTransformIndex != 0
        )
        {
            writer.BaseStream.Position = Header2.SourceBoneTransformIndex;
            foreach (var sbt in SourceBoneTransforms)
                sbt.WriteBinary(writer);
        }

        if (
            Header2 is not null
            && Header2.BoneFlexDriverCount > 0
            && Header2.BoneFlexDriverIndex != 0
        )
        {
            writer.BaseStream.Position = Header.StudioHdr2Index + Header2.BoneFlexDriverIndex;
            foreach (var bfd in BoneFlexDrivers)
                bfd.WriteBinary(writer);
        }

        if (Header.SurfacePropIndex > 0 && !string.IsNullOrEmpty(SurfaceProp))
        {
            writer.BaseStream.Position = Header.SurfacePropIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(SurfaceProp));
            writer.Write((byte)0);
        }

        if (Header.KeyValueIndex > 0 && Header.KeyValueSize > 0)
        {
            writer.BaseStream.Position = Header.KeyValueIndex;
            var kvChars = (KeyValues + '\0').PadRight(Header.KeyValueSize, '\0')[..Header.KeyValueSize];
            writer.Write(kvChars.ToCharArray());
        }

        if (Header.BoneTableByNameIndex > 0 && BoneTableByName != null && BoneTableByName.Length > 0)
        {
            writer.BaseStream.Position = Header.BoneTableByNameIndex;
            writer.Write(BoneTableByName);
        }

        if (writer.BaseStream.Length < Header.Length)
        {
            writer.BaseStream.SetLength(Header.Length);
        }
    }

    private static (List<string> directories, List<int> offsets) ParseTextureDirectories(
        BinaryReader reader,
        StudioMdlHeader header
    )
    {
        if (header.TextureDirectoryCount <= 0 || header.TextureDirectoryIndex <= 0)
        {
            return ([], []);
        }

        var returnPosition = reader.BaseStream.Position;

        reader.BaseStream.Position = header.TextureDirectoryIndex;

        var directories = new List<string>(header.TextureDirectoryCount);
        var offsets = new List<int>(header.TextureDirectoryCount);

        for (var i = 0; i < header.TextureDirectoryCount; i++)
        {
            // cdtextureindex points to an array of absolute file offsets.
            var stringOffset = reader.ReadInt32();
            offsets.Add(stringOffset);
            directories.Add(BinaryReading.ReadStringUntilAt(reader, stringOffset, 0));
        }

        reader.BaseStream.Position = returnPosition;

        return (directories, offsets);
    }

    private static void WriteTextureDirectories(
        BinaryWriter writer,
        StudioMdlHeader header,
        IList<string> directories,
        IList<int>? offsets
    )
    {
        if (header.TextureDirectoryCount <= 0 || header.TextureDirectoryIndex <= 0 || directories.Count == 0)
            return;

        var returnPosition = writer.BaseStream.Position;
        writer.BaseStream.Position = header.TextureDirectoryIndex;

        var currentOffset = (int)(header.TextureDirectoryIndex + header.TextureDirectoryCount * sizeof(int));
        for (var i = 0; i < header.TextureDirectoryCount; i++)
        {
            var offset = (offsets != null && i < offsets.Count) ? offsets[i] : currentOffset;
            writer.Write(offset);
            if (offsets == null || i >= offsets.Count)
            {
                currentOffset += System.Text.Encoding.UTF8.GetByteCount(directories[i]) + 1;
            }
        }

        for (var i = 0; i < header.TextureDirectoryCount; i++)
        {
            var offset = (offsets != null && i < offsets.Count) ? offsets[i] : (int)(header.TextureDirectoryIndex + header.TextureDirectoryCount * sizeof(int));
            if (offsets == null || i >= offsets.Count)
            {
                var prevOffset = (int)(header.TextureDirectoryIndex + header.TextureDirectoryCount * sizeof(int));
                for (var j = 0; j < i; j++)
                    prevOffset += System.Text.Encoding.UTF8.GetByteCount(directories[j]) + 1;
                offset = prevOffset;
            }
            writer.BaseStream.Position = offset;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(directories[i]));
            writer.Write((byte)0);
        }

        writer.BaseStream.Position = returnPosition;
    }

    private static List<int[]> ParseSkins(BinaryReader reader, StudioMdlHeader header)
    {
        if (header.SkinFamilyCount <= 0 || header.SkinRefCount <= 0 || header.SkinIndex <= 0)
        {
            return [];
        }

        var returnPosition = reader.BaseStream.Position;

        reader.BaseStream.Position = header.SkinIndex;

        var rows = new List<int[]>(header.SkinFamilyCount);

        for (var family = 0; family < header.SkinFamilyCount; family++)
        {
            var skin = new int[header.SkinRefCount];

            for (var i = 0; i < header.SkinRefCount; i++)
                skin[i] = reader.ReadInt16();

            rows.Add(skin);
        }

        reader.BaseStream.Position = returnPosition;

        return rows;
    }

    private static void WriteSkins(BinaryWriter writer, StudioMdlHeader header, IList<int[]> skins)
    {
        if (header.SkinFamilyCount <= 0 || header.SkinRefCount <= 0 || header.SkinIndex <= 0 || skins.Count == 0)
        {
            return;
        }

        var returnPosition = writer.BaseStream.Position;
        writer.BaseStream.Position = header.SkinIndex;

        for (var family = 0; family < header.SkinFamilyCount; family++)
        {
            var skin = skins[family];
            for (var i = 0; i < header.SkinRefCount; i++)
                writer.Write((short)skin[i]);
        }

        writer.BaseStream.Position = returnPosition;
    }

    private static string ReadKeyValues(BinaryReader reader, int keyValueIndex, int keyValueSize)
    {
        if (keyValueIndex <= 0 || keyValueSize <= 0)
            return string.Empty;

        var returnPosition = reader.BaseStream.Position;

        reader.BaseStream.Position = keyValueIndex;

        var value = new string(reader.ReadChars(keyValueSize));

        reader.BaseStream.Position = returnPosition;

        return value.TrimEnd('\0');
    }

    private static byte[] ReadBoneTableByName(
        BinaryReader reader,
        int boneTableByNameIndex,
        int boneCount
    )
    {
        if (boneTableByNameIndex <= 0 || boneCount <= 0)
            return [];

        var returnPosition = reader.BaseStream.Position;

        reader.BaseStream.Position = boneTableByNameIndex;

        var value = reader.ReadBytes(boneCount);

        reader.BaseStream.Position = returnPosition;

        return value;
    }

    private static List<T> ReadArray<T>(
        BinaryReader reader,
        int offset,
        int count,
        Func<BinaryReader, T> parser
    )
    {
        reader.BaseStream.Position = offset;

        return Enumerable.Range(0, count).Select(_ => parser(reader)).ToList();
    }
}
