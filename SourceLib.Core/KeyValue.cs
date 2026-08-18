namespace SourceLib.Core;

public sealed class KeyValuePair
{
    public string Key { get; }
    public KeyValueValue Value { get; }
    public IReadOnlyList<string> Tags { get; }

    public KeyValuePair(string key, KeyValueValue value, IReadOnlyList<string>? tags = null)
    {
        Key = key;
        Value = value;
        Tags = tags ?? [];
    }
}

public sealed class KeyValueValue
{
    public ValuePrimitive? Primitive { get; }
    public IReadOnlyList<KeyValuePair>? Children { get; }

    public bool IsPrimitive => Primitive.HasValue;
    public bool IsObject => Children is not null;
    public string? String => Primitive?.String;
    public long? Integer => Primitive?.Integer;
    public double? Float => Primitive?.Float;
    public bool? Boolean => Primitive?.Boolean;
    public IReadOnlyList<KeyValuePair>? Object => Children;

    private KeyValueValue(ValuePrimitive? primitive, IReadOnlyList<KeyValuePair>? children)
    {
        Primitive = primitive;
        Children = children;
    }

    public static KeyValueValue FromPrimitive(ValuePrimitive value) => new(value, null);

    public static KeyValueValue FromObject(IReadOnlyList<KeyValuePair> children) =>
        new(null, children);
}
