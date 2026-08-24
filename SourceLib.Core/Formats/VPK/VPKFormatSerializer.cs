using System.Buffers;
using System.Text;

namespace SourceLib.Core.Formats.VPK;

public sealed class VPKFormatSerializer : IBinaryFormatSerializer<VPK>
{
    public void Serialize(VPK value, IBufferWriter<byte> output)
    {
        if (value.Header is not null)
        {
            WriteUInt32(output, VPKHeader.SIGNATURE);
            WriteUInt32(output, (uint)value.Header.Version);
            WriteUInt32(output, value.Header.TreeSize);

            if (value.Header.Version == VPKVersion.v2)
            {
                WriteUInt32(output, value.Header.FileDataSectionSize ?? 0);
                WriteUInt32(output, value.Header.ArchiveMD5SectionSize ?? 0);
                WriteUInt32(output, value.Header.OtherMD5SectionSize ?? 0);
                WriteUInt32(output, value.Header.SignatureSectionSize ?? 0);
            }
        }

        var extensions = value
            .Files.GroupBy(GetExtension)
            .OrderBy(group => group.Key, StringComparer.Ordinal);

        foreach (var extensionGroup in extensions)
        {
            WriteString(output, extensionGroup.Key);

            var directories = extensionGroup
                .GroupBy(GetDirectory)
                .OrderBy(group => group.Key == " " ? 1 : 0)
                .ThenBy(group => group.Key, StringComparer.Ordinal);

            foreach (var directoryGroup in directories)
            {
                WriteString(output, directoryGroup.Key);

                foreach (var file in directoryGroup.OrderBy(GetFileName, StringComparer.Ordinal))
                {
                    WriteString(output, GetFileName(file));

                    WriteUInt32(output, file.Crc);
                    WriteUInt16(output, file.PreloadSize);

                    foreach (var part in file.Parts)
                    {
                        WriteUInt16(output, part.FileNumber);
                        WriteUInt32(output, part.Offset);
                        WriteUInt32(output, part.Size);
                    }

                    WriteUInt16(output, 0xFFFF);
                    WriteBytes(output, file.PreloadData);
                }

                WriteByte(output, 0);
            }

            WriteByte(output, 0);
        }

        WriteByte(output, 0);
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

    private static void WriteBytes(IBufferWriter<byte> output, IEnumerable<byte> bytes)
    {
        foreach (var value in bytes)
        {
            WriteByte(output, value);
        }
    }
}
