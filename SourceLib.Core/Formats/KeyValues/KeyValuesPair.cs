using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.KeyValues;

public sealed class KeyValuesPair : IKeyValuePair
{
    public string Key { get; }
    public EngineValue? Value { get; }
    public IReadOnlyList<string> Tags { get; }
    public IReadOnlyList<KeyValuesPair>? Object { get; }

    public KeyValuesPair(
        string key,
        EngineValue? value = null,
        IReadOnlyList<string>? tags = null,
        IReadOnlyList<KeyValuesPair>? children = null
    )
    {
        Key = key;
        Value = value;
        Tags = tags ?? [];
        Object = children;
    }
}
