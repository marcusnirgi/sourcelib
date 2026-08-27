namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlTexture
{
    public required int NameIndex { get; set; }
    public required string Name { get; set; }

    public required int Flags { get; set; }

    public required int Used { get; set; }

    public required int Unused1 { get; set; }

    public required int Material { get; set; }

    public required int ClientMaterial { get; set; }

    public required int[] Unused { get; set; }
}
