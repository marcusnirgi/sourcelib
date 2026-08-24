using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.DMX;

public sealed class DmxAttribute
{
    public string Key { get; }
    public DmxTypeIndex TypeIndex { get; }
    public EngineValue Value { get; }
    public DmxElement? ReferencedElement { get; }

    public DmxAttribute(
        string key,
        DmxTypeIndex typeIndex,
        EngineValue value,
        DmxElement? referencedElement = null
    )
    {
        Key = key;
        TypeIndex = typeIndex;
        Value = value;
        ReferencedElement = referencedElement;
    }
}
