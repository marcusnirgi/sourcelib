namespace SourceLib.Core;

public enum ValuePrimitiveType
{
    String,
    Integer,
    Float,
    Boolean,
    Object,
}

public readonly record struct ValuePrimitive
{
    public ValuePrimitiveType Type { get; }
    public string? String { get; }
    public long Integer { get; }
    public double Float { get; }
    public bool Boolean { get; }

    private ValuePrimitive(
        ValuePrimitiveType type,
        string? @string = null,
        long integer = 0,
        double @float = 0,
        bool boolean = false
    )
    {
        Type = type;
        String = @string;
        Integer = integer;
        Float = @float;
        Boolean = boolean;
    }

    public static ValuePrimitive FromString(string value) =>
        new(ValuePrimitiveType.String, @string: value);

    public static ValuePrimitive FromInteger(long value) =>
        new(ValuePrimitiveType.Integer, integer: value);

    public static ValuePrimitive FromFloat(double value) =>
        new(ValuePrimitiveType.Float, @float: value);

    public static ValuePrimitive FromBoolean(bool value) =>
        new(ValuePrimitiveType.Boolean, boolean: value);
}
