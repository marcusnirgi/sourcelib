using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlFlexDesc
{
    public required int FacsIndex { get; set; }
    public required string Name { get; set; }
}

public sealed class StudioMdlFlexController
{
    public required int TypeIndex { get; set; }
    public required int NameIndex { get; set; }
    public required int LocalToGlobal { get; set; }
    public required float Min { get; set; }
    public required float Max { get; set; }

    public required string Name { get; set; }
    public required string Type { get; set; }
}

public sealed class StudioMdlFlexControllerUi
{
    public required int NameIndex { get; set; }
    public required int Index0 { get; set; }
    public required int Index1 { get; set; }
    public required int Index2 { get; set; }
    public required byte RemapType { get; set; }
    public required byte Stereo { get; set; }
    public required byte[] Unused { get; set; }

    public required string Name { get; set; }
}

public sealed class StudioMdlFlexOp
{
    public required int Op { get; set; }
    public required int D { get; set; }
}

public sealed class StudioMdlFlexRule
{
    public required int Flex { get; set; }
    public required int NumOps { get; set; }
    public required int OpIndex { get; set; }
    public required IList<StudioMdlFlexOp> Ops { get; set; }
}

public sealed class StudioMdlFlex
{
    public required int FlexDesc { get; set; }
    public required float Target0 { get; set; }
    public required float Target1 { get; set; }
    public required float Target2 { get; set; }
    public required float Target3 { get; set; }
    public required int NumVerts { get; set; }
    public required int VertIndex { get; set; }
    public required int FlexPair { get; set; }
    public required byte VertAnimType { get; set; }
    public required byte[] UnusedChar { get; set; }
    public required int[] Unused { get; set; }
}

public sealed class StudioMdlVertAnim
{
    public required ushort Index { get; set; }
    public required byte Speed { get; set; }
    public required byte Side { get; set; }
    public required short[] Delta { get; set; }
    public required short[] NormalDelta { get; set; }
}

public sealed class StudioMdlVertAnimWrinkle
{
    public required ushort Index { get; set; }
    public required byte Speed { get; set; }
    public required byte Side { get; set; }
    public required short[] Delta { get; set; }
    public required short[] NormalDelta { get; set; }
    public required short WrinkleDelta { get; set; }
}

public sealed class StudioMdlEyeball
{
    public required int NameIndex { get; set; }
    public required int Bone { get; set; }
    public required Vector3 Org { get; set; }
    public required float ZOffset { get; set; }
    public required float Radius { get; set; }
    public required Vector3 Up { get; set; }
    public required Vector3 Forward { get; set; }
    public required int Texture { get; set; }
    public required int Unused1 { get; set; }
    public required float IrisScale { get; set; }
    public required int Unused2 { get; set; }
    public required int[] UpperFlexDesc { get; set; }
    public required int[] LowerFlexDesc { get; set; }
    public required float[] UpperTarget { get; set; }
    public required float[] LowerTarget { get; set; }
    public required int UpperLidFlexDesc { get; set; }
    public required int LowerLidFlexDesc { get; set; }
    public required int[] Unused { get; set; }
    public required byte NonFacs { get; set; }
    public required byte[] Unused3 { get; set; }
    public required int[] Unused4 { get; set; }

    public required string Name { get; set; }
}