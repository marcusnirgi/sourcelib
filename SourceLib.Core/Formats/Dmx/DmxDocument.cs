namespace SourceLib.Core.Formats.Dmx;

public sealed class DmxDocument
{
    public string Header { get; set; }
    public IEnumerable<DmxElement> Elements { get; set; }

    public DmxDocument(string header, IEnumerable<DmxElement> elements)
    {
        Header = header;
        Elements = elements;
    }
}
