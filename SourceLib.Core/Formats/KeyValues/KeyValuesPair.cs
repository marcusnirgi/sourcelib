namespace SourceLib.Core.Formats.KeyValues;

public sealed class KeyValuesPair : IKeyValuePair
{
    public string Key { get; }
    public KeyValueValue Value { get; }
    public IReadOnlyList<string> Tags { get; }
    public IReadOnlyList<KeyValuesPair>? Children { get; }
    public IReadOnlyList<KeyValueValue>? Array { get; }

    public KeyValuesPair(
        string key,
        KeyValueValue value,
        IReadOnlyList<string>? tags = null,
        IReadOnlyList<KeyValuesPair>? children = null,
        IReadOnlyList<KeyValueValue>? array = null
    )
    {
        Key = key;
        Value = value;
        Tags = tags ?? [];
        Children = children;
        Array = array;
    }
}
