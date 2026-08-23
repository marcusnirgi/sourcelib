namespace SourceLib.Core.Formats.Dmx;

public sealed class DmxElement
{
    public string ClassName { get; set; }
    public string Name { get; set; }
    public Guid Id { get; set; }
    public IEnumerable<DmxAttribute> Attributes { get; set; }

    public DmxElement(
        string className,
        string name,
        Guid id,
        IEnumerable<DmxAttribute>? attributes = null
    )
    {
        ClassName = className;
        Name = name;
        Id = id;
        Attributes = attributes ?? [];
    }
}
