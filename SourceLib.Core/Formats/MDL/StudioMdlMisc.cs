namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlPoseParamDesc
{
    public required int NameIndex { get; set; }
    public required int Flags { get; set; }
    public required float Start { get; set; }
    public required float End { get; set; }
    public required float Loop { get; set; }
    public required string Name { get; set; }
}

public sealed class StudioMdlMouth
{
    public required int Bone { get; set; }
    public required float ForwardX { get; set; }
    public required float ForwardY { get; set; }
    public required float ForwardZ { get; set; }
    public required int FlexDesc { get; set; }
}

public sealed class StudioMdlModelGroup
{
    public required int LabelIndex { get; set; }
    public required int NameIndex { get; set; }
    public required string Label { get; set; }
    public required string Name { get; set; }
}

public sealed class StudioMdlAnimBlock
{
    public required int DataStart { get; set; }
    public required int DataEnd { get; set; }
}

public sealed class StudioMdlActivityModifier
{
    public required int NameIndex { get; set; }
    public required string Name { get; set; }
}