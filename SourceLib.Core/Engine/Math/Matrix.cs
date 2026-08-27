namespace SourceLib.Core.Engine.Math;

public sealed class Matrix3x4
{
    public Matrix3x4(IList<float> values)
    {
        if (values.Count != 12)
            throw new ArgumentException("Matrix requires exactly 12 values");

        Values = [.. values];
    }

    public IList<float> Values { get; }

    public static Matrix3x4 ReadBinary(BinaryReader reader)
    {
        return new Matrix3x4([
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
        ]);
    }

    public void WriteBinary(BinaryWriter writer)
    {
        foreach (var value in Values)
        {
            writer.Write(value);
        }
    }
}

public sealed class Matrix4x4
{
    public Matrix4x4(IList<float> values)
    {
        if (values.Count != 16)
            throw new ArgumentException("Matrix requires exactly 16 values");

        Values = [.. values];
    }

    public IList<float> Values { get; }

    public static Matrix4x4 ReadBinary(BinaryReader reader)
    {
        return new Matrix4x4([
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
        ]);
    }

    public void WriteBinary(BinaryWriter writer)
    {
        foreach (var value in Values)
        {
            writer.Write(value);
        }
    }
}
