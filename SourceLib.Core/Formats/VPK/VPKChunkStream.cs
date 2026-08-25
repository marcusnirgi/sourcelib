namespace SourceLib.Core.Formats.VPK;

public sealed class VPKChunkStream : MemoryStream
{
    public VPKChunkStream(byte[] data)
    {
        Write(data);
        Position = 0;
    }
}
