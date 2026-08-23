using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.Dmx;

public sealed class DmxAttribute
{
    public string Key { get; set; }
    public DmxTypeIndex TypeIndex { get; set; }
    public EngineValue Value { get; set; }

    public DmxAttribute(string key, DmxTypeIndex typeIndex, EngineValue value)
    {
        Key = key;
        TypeIndex = typeIndex;
        Value = value;
    }
}
