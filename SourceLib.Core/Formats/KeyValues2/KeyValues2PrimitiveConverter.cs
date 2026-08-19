namespace SourceLib.Core.Formats.KeyValues2;

public static class KeyValues2PrimitiveConverter
{
    public static ValuePrimitive ToPrimitive(string typeHint, string value)
    {
        return typeHint switch
        {
            KeyValues2TypeHint.Bool => ValuePrimitive.FromBoolean(value == "1"),
            KeyValues2TypeHint.Int => ValuePrimitive.FromInteger(long.Parse(value)),
            KeyValues2TypeHint.Float => ValuePrimitive.FromFloat(double.Parse(value)),
            KeyValues2TypeHint.String => ValuePrimitive.FromString(value),
            KeyValues2TypeHint.ElementId => ValuePrimitive.FromString(value),
            KeyValues2TypeHint.Quaternion => ValuePrimitive.FromString(value),
            KeyValues2TypeHint.Vector3 => ValuePrimitive.FromString(value),
            KeyValues2TypeHint.Element => ValuePrimitive.FromString(value),
            KeyValues2TypeHint.Color => ValuePrimitive.FromString(value),
            _ => throw new NotSupportedException($"Unsupported KV2 primitive type '{typeHint}'"),
        };
    }
}
