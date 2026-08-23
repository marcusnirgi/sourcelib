using System.Text.Json.Serialization;
using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Engine;

[JsonConverter(typeof(EngineValueJsonConverter))]
public abstract class EngineValue;

public class EngineValue<TAnyValueType> : EngineValue
{
    public EngineValue(TAnyValueType value) => Value = value;

    public TAnyValueType Value { get; }
}

public sealed class EngineByte : EngineValue<byte>
{
    public EngineByte(byte value)
        : base(value) { }
}

public class EngineArray<TValueType> : EngineValue
    where TValueType : EngineValue
{
    public EngineArray(IReadOnlyList<TValueType> values) => Values = values;

    public IReadOnlyList<TValueType> Values { get; }
}

public sealed class EngineInt : EngineValue<int>
{
    public EngineInt(int value)
        : base(value) { }
}

public sealed class EngineFloat : EngineValue<float>
{
    public EngineFloat(float value)
        : base(value) { }
}

public sealed class EngineBool : EngineValue<bool>
{
    public EngineBool(bool value)
        : base(value) { }
}

public sealed class EngineString : EngineValue<string>
{
    public EngineString(string value)
        : base(value) { }
}

public sealed class EngineGuid : EngineValue<Guid>
{
    public EngineGuid(Guid value)
        : base(value) { }
}

public sealed class EngineBytes : EngineValue
{
    public EngineBytes(ReadOnlyMemory<byte> value) => Value = value;

    public ReadOnlyMemory<byte> Value { get; }
}

public sealed class EngineByteArray : EngineValue
{
    public EngineByteArray(IReadOnlyList<ReadOnlyMemory<byte>> values) => Values = values;

    public IReadOnlyList<ReadOnlyMemory<byte>> Values { get; }
}

public sealed class EngineVector2 : EngineValue<Vector2>
{
    public EngineVector2(Vector2 value)
        : base(value) { }

    public EngineVector2(float x, float y)
        : base(new Vector2(x, y)) { }
}

public sealed class EngineVector3 : EngineValue<Vector3>
{
    public EngineVector3(Vector3 value)
        : base(value) { }

    public EngineVector3(float x, float y, float z)
        : base(new Vector3(x, y, z)) { }
}

public sealed class EngineVector4 : EngineValue<Vector4>
{
    public EngineVector4(Vector4 value)
        : base(value) { }

    public EngineVector4(float x, float y, float z, float w)
        : base(new Vector4(x, y, z, w)) { }
}

public sealed class EngineAngle : EngineValue<Angle>
{
    public EngineAngle(Angle value)
        : base(value) { }

    public EngineAngle(float pitch, float yaw, float roll)
        : base(new Angle(pitch, yaw, roll)) { }
}

public sealed class EngineQuaternion : EngineValue<Quaternion>
{
    public EngineQuaternion(Quaternion value)
        : base(value) { }

    public EngineQuaternion(float x, float y, float z, float w)
        : base(new Quaternion(x, y, z, w)) { }
}

public sealed class EngineColor3 : EngineValue<Color3>
{
    public EngineColor3(Color3 value)
        : base(value) { }

    public EngineColor3(float r, float g, float b)
        : base(new Color3(r, g, b)) { }
}

public sealed class EngineColor4 : EngineValue<Color4>
{
    public EngineColor4(Color4 value)
        : base(value) { }

    public EngineColor4(float r, float g, float b, float alpha)
        : base(new Color4(r, g, b, alpha)) { }
}

public sealed class EngineTime : EngineValue<Time>
{
    public EngineTime(Time value)
        : base(value) { }

    public EngineTime(float seconds)
        : base(new Time(seconds)) { }
}

public sealed class EngineMatrix : EngineValue<Matrix>
{
    public EngineMatrix(Matrix value)
        : base(value) { }

    public EngineMatrix(IReadOnlyList<float> values)
        : base(new Matrix(values)) { }
}
