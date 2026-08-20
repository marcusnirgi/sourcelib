namespace SourceLib.Core.Formats.KeyValues3;

public sealed class KeyValues3Pair : IKeyValuePair
{
    public string Key { get; }
    public KeyValueValue Value { get; }
    public IReadOnlyList<KeyValues3Pair>? Children { get; }
    public IReadOnlyList<KeyValues3ArrayValue>? Array { get; }

    public KeyValues3Pair(
        string key,
        KeyValueValue value,
        IReadOnlyList<KeyValues3Pair>? children = null,
        IReadOnlyList<KeyValues3ArrayValue>? array = null
    )
    {
        Key = key;
        Value = value;
        Children = children;
        Array = array;
    }
}

public sealed class KeyValues3ArrayValue
{
    public KeyValueValue Value { get; }
    public IReadOnlyList<KeyValues3Pair>? Children { get; }
    public IReadOnlyList<KeyValues3ArrayValue>? Array { get; }

    private KeyValues3ArrayValue(
        KeyValueValue value,
        IReadOnlyList<KeyValues3Pair>? children = null,
        IReadOnlyList<KeyValues3ArrayValue>? array = null
    )
    {
        Value = value;
        Children = children;
        Array = array;
    }

    public static KeyValues3ArrayValue FromValue(
        KeyValueValue value,
        IReadOnlyList<KeyValues3Pair>? children = null,
        IReadOnlyList<KeyValues3ArrayValue>? array = null
    )
    {
        return new KeyValues3ArrayValue(value, children, array);
    }
}
