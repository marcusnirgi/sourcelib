namespace SourceLib.Core.Formats.KeyValues2;

public sealed class KeyValues2Pair : IKeyValuePair
{
    public string Key { get; }
    public KeyValueValue Value { get; }
    public string? TypeHint { get; }
    public IReadOnlyList<KeyValues2Pair>? Children { get; }
    public IReadOnlyList<KeyValues2ArrayValue>? Array { get; }

    public KeyValues2Pair(
        string key,
        KeyValueValue value,
        string? typeHint = null,
        IReadOnlyList<KeyValues2Pair>? children = null,
        IReadOnlyList<KeyValues2ArrayValue>? array = null
    )
    {
        Key = key;
        Value = value;
        TypeHint = typeHint;
        Children = children;
        Array = array;
    }
}

public sealed class KeyValues2ArrayValue
{
    public KeyValueValue Value { get; }
    public string? TypeHint { get; }

    private KeyValues2ArrayValue(KeyValueValue value, string? typeHint = null)
    {
        Value = value;
        TypeHint = typeHint;
    }

    public static KeyValues2ArrayValue FromValue(KeyValueValue value, string? typeHint = null)
    {
        return new KeyValues2ArrayValue(value, typeHint);
    }
}
