namespace SourceLib.Core.Formats.MDL;

[Flags]
public enum StudioMdlBoneFlags
{
    PhysicallySimulated = 0x00000001,
    PhysicsProcedural = 0x00000002,
    AlwaysProcedural = 0x00000004,
    ScreenAlignSphere = 0x00000008,
    ScreenAlignCylinder = 0x00000010,

    UsedByHitbox = 0x00000100,
    UsedByAttachment = 0x00000200,

    UsedByVertexLod0 = 0x00000400,
    UsedByVertexLod1 = 0x00000800,
    UsedByVertexLod2 = 0x00001000,
    UsedByVertexLod3 = 0x00002000,
    UsedByVertexLod4 = 0x00004000,
    UsedByVertexLod5 = 0x00008000,
    UsedByVertexLod6 = 0x00010000,
    UsedByVertexLod7 = 0x00020000,

    UsedByBoneMerge = 0x00040000,

    FixedAlignment = 0x00100000,
    HasSaveFramePos = 0x00200000,
    HasSaveFrameRot = 0x00400000,
}
