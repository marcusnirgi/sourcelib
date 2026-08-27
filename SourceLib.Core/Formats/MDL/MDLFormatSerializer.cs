namespace SourceLib.Core.Formats.MDL;

public sealed class MDLFormatSerializer : IBinaryFormatSerializer<StudioMdl>
{
    public byte[] Serialize(StudioMdl value)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        value.WriteBinary(writer);
        return stream.ToArray();
    }
}
