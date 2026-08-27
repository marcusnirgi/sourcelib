namespace SourceLib.Core.Formats.MDL;

public sealed class MDLFormatParser : IBinaryFormatParser<StudioMdl>
{
    public StudioMdl Parse(byte[] data)
    {
        using var stream = new MemoryStream(data.ToArray());
        using var reader = new BinaryReader(stream);
        return StudioMdl.ReadBinary(reader);
    }
}
