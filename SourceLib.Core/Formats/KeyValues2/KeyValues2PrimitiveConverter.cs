using SourceLib.Core.Engine;
using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.KeyValues2;

public static class KeyValues2EngineValueConverter
{
    public static EngineValue ToPrimitive(string typeHint, string value)
    {
        return typeHint switch
        {
            KeyValues2TypeHint.Bool => new EngineBool(value == "1"),
            KeyValues2TypeHint.Int => new EngineInt(int.Parse(value)),
            KeyValues2TypeHint.Float => new EngineFloat(float.Parse(value)),
            KeyValues2TypeHint.String => new EngineString(value),
            KeyValues2TypeHint.ElementId => new EngineGuid(Guid.Parse(value)),
            KeyValues2TypeHint.Element => new EngineGuid(Guid.Parse(value)),
            KeyValues2TypeHint.Binary => new EngineBytes(Convert.FromHexString(value)),
            KeyValues2TypeHint.Time => new EngineTime(float.Parse(value)),
            KeyValues2TypeHint.Color => new EngineColor4(ParseColor(value)),
            KeyValues2TypeHint.Vector2 => new EngineVector2(ParseVector2(value)),
            KeyValues2TypeHint.Vector3 => new EngineVector3(ParseVector3(value)),
            KeyValues2TypeHint.Vector4 => new EngineVector4(ParseVector4(value)),
            KeyValues2TypeHint.Angle => new EngineAngle(ParseAngle(value)),
            KeyValues2TypeHint.Quaternion => new EngineQuaternion(ParseQuaternion(value)),
            KeyValues2TypeHint.Matrix => new EngineMatrix(ParseMatrix(value)),
            _ => throw new NotSupportedException($"Unsupported KV2 primitive type '{typeHint}'"),
        };
    }

    private static Vector2 ParseVector2(string value)
    {
        var parts = value.Split(' ');

        return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
    }

    private static Vector3 ParseVector3(string value)
    {
        var parts = value.Split(' ');

        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }

    private static Vector4 ParseVector4(string value)
    {
        var parts = value.Split(' ');

        return new Vector4(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2]),
            float.Parse(parts[3])
        );
    }

    private static Angle ParseAngle(string value)
    {
        var parts = value.Split(' ');

        return new Angle(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }

    private static Quaternion ParseQuaternion(string value)
    {
        var parts = value.Split(' ');

        return new Quaternion(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2]),
            float.Parse(parts[3])
        );
    }

    private static Color4 ParseColor(string value)
    {
        var parts = value.Split(' ');

        return new Color4(
            byte.Parse(parts[0]),
            byte.Parse(parts[1]),
            byte.Parse(parts[2]),
            byte.Parse(parts[3])
        );
    }

    private static Matrix ParseMatrix(string value)
    {
        var parts = value.Split(' ');

        var values = new float[16];

        for (var i = 0; i < 16; i++)
            values[i] = float.Parse(parts[i]);

        return new Matrix(values);
    }
}
