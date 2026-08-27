using System.Text;
using SourceLib.Core.Engine.Math;

namespace SourceLib.Core;

public static class BinaryReading
{
    public static ReadOnlyMemory<byte> ReadBytesUntil(BinaryReader reader, byte delimiter)
    {
        var bytes = new List<byte>();

        while (true)
        {
            var value = reader.ReadByte();

            if (value == delimiter)
                break;

            bytes.Add(value);
        }

        return new ReadOnlyMemory<byte>(bytes.ToArray());
    }

    public static string ReadStringUntil(BinaryReader reader, byte delimiter)
    {
        var bytes = ReadBytesUntil(reader, delimiter);
        return Encoding.UTF8.GetString(bytes.Span);
    }

    public static Vector3 ReadVector3(BinaryReader reader)
    {
        return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public static Quaternion ReadQuaternion(BinaryReader reader)
    {
        return new Quaternion(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    public static Matrix ReadMatrix(BinaryReader reader)
    {
        return new Matrix([
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
}
