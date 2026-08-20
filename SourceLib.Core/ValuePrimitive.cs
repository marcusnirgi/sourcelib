using System.Globalization;

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

    public static ValuePrimitive InferFromString(string value)
    {
        if (value.Length == 0)
        {
            return FromString(value);
        }

        if (long.TryParse(value, out var longValue))
        {
            return FromInteger(longValue);
        }

        if (double.TryParse(value, out var floatValue))
        {
            return FromFloat(floatValue);
        }

        if (bool.TryParse(value, out var boolValue))
        {
            return FromBoolean(boolValue);
        }

        return FromString(value);
    }
}

public static class ValuePrimitiveFormatter
{
    public static string FormatFloat(double value)
    {
        var text = value.ToString("R", CultureInfo.InvariantCulture);

        if (!text.Contains('.') && !text.Contains('e') && !text.Contains('E'))
        {
            text += ".0";
        }

        return text;
    }
}
