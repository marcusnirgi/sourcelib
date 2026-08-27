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
        byte[] boneTableByName
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
    }

    public StudioMdlTexture GetTexture(int skinIndex, int materialIndex)
    {
        if (skinIndex < 0 || skinIndex >= Skins.Count)
        {
            throw new IndexOutOfRangeException($"Skin index {skinIndex} is out of range.");
        }

        if (materialIndex < 0 || materialIndex >= Skins[skinIndex].Length)
        {
            throw new IndexOutOfRangeException($"Material index {materialIndex} is out of range.");
        }

        var textureIndex = Skins[skinIndex][materialIndex];

        if (textureIndex < 0 || textureIndex >= Textures.Count)
        {
            throw new IndexOutOfRangeException($"Texture index {textureIndex} is out of range.");
        }

        return Textures[textureIndex];
    }
}