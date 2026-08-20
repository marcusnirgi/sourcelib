namespace SourceLib.Core.Formats.KeyValues3;

public sealed class KeyValues3Pair : IKeyValuePair
{
    public string Key { get; }
    public KeyValueValue Value { get; }
    public IReadOnlyList<KeyValues3Pair>? Children { get; }
    public IReadOnlyList<KeyValueValue>? Array { get; }

    public KeyValues3Pair(
        string key,
        KeyValueValue value,
        IReadOnlyList<KeyValues3Pair>? children = null,
        IReadOnlyList<KeyValueValue>? array = null
    )
    {
        Key = key;
        Value = value;
        Children = children;
        Array = array;
    }
}
