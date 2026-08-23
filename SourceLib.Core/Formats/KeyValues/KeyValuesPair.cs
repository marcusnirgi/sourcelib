namespace SourceLib.Core.Formats.KeyValues;

public sealed class KeyValuesPair : IKeyValuePair
{
    public string Key { get; }
    public ValuePrimitive Value { get; }
    public IReadOnlyList<string> Tags { get; }
    public IReadOnlyList<KeyValuesPair>? Children { get; }
    public IReadOnlyList<ValuePrimitive>? Array { get; }

    public KeyValuesPair(
        string key,
        ValuePrimitive value,
        IReadOnlyList<string>? tags = null,
        IReadOnlyList<KeyValuesPair>? children = null,
        IReadOnlyList<ValuePrimitive>? array = null
    )
    {
        Key = key;
        Value = value;
        Tags = tags ?? [];
        Children = children;
        Array = array;
    }
}
