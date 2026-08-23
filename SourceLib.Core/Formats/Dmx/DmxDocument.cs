namespace SourceLib.Core.Formats.Dmx;

public sealed class DmxDocument
{
    public DmxHeader Header { get; set; }
    public IEnumerable<DmxElement> Elements { get; set; }

    public DmxDocument(DmxHeader header, IEnumerable<DmxElement> elements)
    {
        Header = header;
        Elements = elements;
    }
}
