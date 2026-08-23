using System.Text;

namespace SourceLib.Core;

public static class BinaryHelper
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
}
