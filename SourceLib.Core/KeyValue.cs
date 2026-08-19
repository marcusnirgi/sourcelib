namespace SourceLib.Core;

public interface IKeyValuePair
{
    string Key { get; }
    KeyValueValue Value { get; }
}

public class KeyValueValue
{
    public ValuePrimitive? Primitive { get; }

    public string? String => Primitive?.String;
    public long? Integer => Primitive?.Integer;
    public double? Float => Primitive?.Float;
    public bool? Boolean => Primitive?.Boolean;

    private KeyValueValue(ValuePrimitive? primitive)
    {
        Primitive = primitive;
    }

    public static KeyValueValue FromPrimitive(ValuePrimitive value) => new(value);
}
