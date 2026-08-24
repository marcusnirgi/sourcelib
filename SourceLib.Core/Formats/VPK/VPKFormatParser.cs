using System.Collections.Immutable;

namespace SourceLib.Core.Formats.VPK;

public sealed class VPKFormatParser : IBinaryFormatParser<VPK>
{
    public VPK Parse(ReadOnlySpan<byte> input)
    {
        using var stream = new MemoryStream(input.ToArray());
        using var reader = new BinaryReader(stream);

        var files = new List<VPKFile>();

        while (true)
        {
            var extensionName = BinaryHelper.ReadStringUntil(reader, 0);

            if (extensionName.Length == 0)
                break;

            while (true)
            {
                var directoryName = BinaryHelper.ReadStringUntil(reader, 0);

                if (directoryName.Length == 0)
                    break;

                while (true)
                {
                    var fileName = BinaryHelper.ReadStringUntil(reader, 0);

                    if (fileName.Length == 0)
                        break;

                    var crc = reader.ReadUInt32();
                    var metaDataSize = reader.ReadUInt16();

                    var parts = new List<VPKFilePart>();

                    while (true)
                    {
                        var fileNumber = reader.ReadUInt16();

                        if (fileNumber == 0xFFFF)
                            break;

                        var fileDataOffset = reader.ReadUInt32();
                        var fileDataSize = reader.ReadUInt32();

                        parts.Add(
                            new VPKFilePart
                            {
                                FileNumber = fileNumber,
                                Offset = fileDataOffset,
                                Size = fileDataSize,
                            }
                        );
                    }

                    var metadata = reader.ReadBytes(metaDataSize);

                    var path = string.IsNullOrWhiteSpace(directoryName)
                        ? $"{fileName}.{extensionName}"
                        : $"{directoryName}/{fileName}.{extensionName}";

                    var file = new VPKFile
                    {
                        Path = path,
                        Crc = crc,
                        PreloadSize = metaDataSize,
                        PreloadData = metadata,
                        Parts = parts.ToImmutableList(),
                    };
                    files.Add(file);
                }
            }
        }

        return new VPK() { Files = files.ToImmutableList() };
    }
}
