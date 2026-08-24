using System.Buffers;
using System.Text;

namespace SourceLib.Core.Formats.VPK;

public sealed class VPKFormatSerializer : BinaryFormatSerializer<VPK>
{
    public override byte[] Serialize(VPK value)
    {
        var writer = new ArrayBufferWriter<byte>();
        var tree = new ArrayBufferWriter<byte>();

        var extensions = value
            .Files.GroupBy(GetExtension)
            .OrderBy(group => group.Key, StringComparer.Ordinal);

        foreach (var extensionGroup in extensions)
        {
            WriteString(tree, extensionGroup.Key);

            var directories = extensionGroup
                .GroupBy(GetDirectory)
                .OrderBy(group => group.Key == " " ? 1 : 0)
                .ThenBy(group => group.Key, StringComparer.Ordinal);

            foreach (var directoryGroup in directories)
            {
                WriteString(tree, directoryGroup.Key);

                foreach (var file in directoryGroup.OrderBy(GetFileName, StringComparer.Ordinal))
                {
                    WriteString(tree, GetFileName(file));

                    WriteUInt32(tree, file.Crc);
                    WriteUInt16(tree, file.PreloadSize);

                    foreach (var part in file.Parts)
                    {
                        WriteUInt16(tree, part.FileNumber);
                        WriteUInt32(tree, part.Offset);
                        WriteUInt32(tree, part.Size);
                    }

                    WriteUInt16(tree, 0xFFFF);
                    WriteBytes(tree, file.PreloadData);
                }

                WriteByte(tree, 0);
            }

            WriteByte(tree, 0);
        }

        WriteByte(tree, 0);

        if (value.Header is null)
        {
            return tree.WrittenMemory.ToArray();
        }

        var header = value.Header;
        var treeSize = checked((uint)tree.WrittenCount);

        WriteUInt32(writer, VPKHeader.SIGNATURE);
        WriteUInt32(writer, (uint)header.Version);
        WriteUInt32(writer, treeSize);

        if (header.Version == VPKVersion.v2)
        {
            WriteUInt32(writer, checked((uint)header.FileDataSection.Length));
            WriteUInt32(writer, checked((uint)header.ArchiveMD5Section.Length));
            WriteUInt32(writer, checked((uint)header.OtherMD5Section.Length));
            WriteUInt32(writer, checked((uint)header.SignatureSection.Length));
        }

        WriteBytes(writer, tree.WrittenSpan);

        if (header.Version == VPKVersion.v2)
        {
            WriteBytes(writer, header.FileDataSection);
            WriteBytes(writer, header.ArchiveMD5Section);
            WriteBytes(writer, header.OtherMD5Section);
            WriteBytes(writer, header.SignatureSection);
        }

        return writer.WrittenMemory.ToArray();
    }

    private static string GetExtension(VPKFile file)
    {
        var path = file.Path.Replace('\\', '/');
        var fileNameStart = path.LastIndexOf('/') + 1;
        var extensionStart = path.LastIndexOf('.');

        if (extensionStart < fileNameStart)
        {
            throw new InvalidDataException($"VPK file '{file.Path}' has no extension.");
        }

        return path[(extensionStart + 1)..];
    }

    private static string GetDirectory(VPKFile file)
    {
        var path = file.Path.Replace('\\', '/');
        var separator = path.LastIndexOf('/');

        return separator < 0 ? " " : path[..separator];
    }

    private static string GetFileName(VPKFile file)
    {
        var path = file.Path.Replace('\\', '/');
        var fileNameStart = path.LastIndexOf('/') + 1;
        var extensionStart = path.LastIndexOf('.');

        if (extensionStart < fileNameStart)
        {
            throw new InvalidDataException($"VPK file '{file.Path}' has no extension.");
        }

        return path[fileNameStart..extensionStart];
    }

    private static void WriteString(IBufferWriter<byte> output, string value)
    {
        var byteCount = Encoding.ASCII.GetByteCount(value);
        var span = output.GetSpan(byteCount + 1);

        Encoding.ASCII.GetBytes(value, span);
        span[byteCount] = 0;

        output.Advance(byteCount + 1);
    }

    private static void WriteByte(IBufferWriter<byte> output, byte value)
    {
        var span = output.GetSpan(1);
        span[0] = value;

        output.Advance(1);
    }

    private static void WriteUInt16(IBufferWriter<byte> output, ushort value)
    {
        var span = output.GetSpan(2);

        span[0] = (byte)value;
        span[1] = (byte)(value >> 8);

        output.Advance(2);
    }

    private static void WriteUInt32(IBufferWriter<byte> output, uint value)
    {
        var span = output.GetSpan(4);

        span[0] = (byte)value;
        span[1] = (byte)(value >> 8);
        span[2] = (byte)(value >> 16);
        span[3] = (byte)(value >> 24);

        output.Advance(4);
    }

    private static void WriteBytes(IBufferWriter<byte> output, ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty)
        {
            return;
        }

        var span = output.GetSpan(bytes.Length);
        bytes.CopyTo(span);

        output.Advance(bytes.Length);
    }

    private static void WriteBytes(IBufferWriter<byte> output, IEnumerable<byte> bytes)
    {
        foreach (var value in bytes)
        {
            WriteByte(output, value);
        }
    }
}
