namespace SourceLib.Core.Formats.VPK;

public sealed class VPKFormatParser
{
    public VPK Parse(ReadOnlySpan<byte> directory, IReadOnlyList<Stream> chunks)
    {
        using var stream = new MemoryStream(directory.ToArray());
        using var reader = new BinaryReader(stream);

        VPKHeader? header = null;

        var signature = reader.ReadUInt32();

        if (signature == VPKHeader.SIGNATURE)
        {
            var version = (VPKVersion)reader.ReadUInt32();
            var treeSize = reader.ReadUInt32();

            uint? fileDataSectionSize = null;
            uint? archiveMD5SectionSize = null;
            uint? otherMD5SectionSize = null;
            uint? signatureSectionSize = null;

            byte[] fileDataSection = [];
            byte[] archiveMD5Section = [];
            byte[] otherMD5Section = [];
            byte[] signatureSection = [];

            if (version == VPKVersion.v2)
            {
                fileDataSectionSize = reader.ReadUInt32();
                archiveMD5SectionSize = reader.ReadUInt32();
                otherMD5SectionSize = reader.ReadUInt32();
                signatureSectionSize = reader.ReadUInt32();
            }
            else if (version != VPKVersion.v1)
            {
                throw new InvalidDataException($"Unknown header version {version}");
            }

            header = new VPKHeader
            {
                Signature = signature,
                Version = version,
                TreeSize = treeSize,
                FileDataSectionSize = fileDataSectionSize,
                ArchiveMD5SectionSize = archiveMD5SectionSize,
                OtherMD5SectionSize = otherMD5SectionSize,
                SignatureSectionSize = signatureSectionSize,
            };
        }
        else
        {
            reader.BaseStream.Position = 0;
        }

        var treeStart = reader.BaseStream.Position;

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

                    if (metadata.Length != metaDataSize)
                    {
                        throw new EndOfStreamException(
                            $"Unexpected end of metadata for VPK file '{fileName}'."
                        );
                    }

                    var path = string.IsNullOrWhiteSpace(directoryName)
                        ? $"{fileName}.{extensionName}"
                        : $"{directoryName}/{fileName}.{extensionName}";

                    files.Add(
                        new VPKFile
                        {
                            Path = path,
                            Crc = crc,
                            PreloadSize = metaDataSize,
                            PreloadData = metadata.ToList(),
                            Parts = parts,
                        }
                    );
                }
            }
        }

        var treeBytesRead = checked(reader.BaseStream.Position - treeStart);

        if (header is not null)
        {
            var expectedTreeSize = header.TreeSize;

            if (header.Version == VPKVersion.v2)
            {
                var sectionSize = checked(
                    header.FileDataSectionSize.GetValueOrDefault()
                    + header.ArchiveMD5SectionSize.GetValueOrDefault()
                    + header.OtherMD5SectionSize.GetValueOrDefault()
                    + header.SignatureSectionSize.GetValueOrDefault()
                );

                if (expectedTreeSize == treeBytesRead + sectionSize)
                {
                    expectedTreeSize = checked(expectedTreeSize - sectionSize);
                }
            }

            if (treeBytesRead != expectedTreeSize)
            {
                throw new InvalidDataException(
                    $"VPK tree size mismatch. Header specifies {expectedTreeSize} bytes, "
                        + $"but {treeBytesRead} bytes were read."
                );
            }

            if (header.Version == VPKVersion.v2)
            {
                var fileDataSection = ReadSection(
                    reader,
                    header.FileDataSectionSize.GetValueOrDefault(),
                    "file data"
                );

                var archiveMD5Section = ReadSection(
                    reader,
                    header.ArchiveMD5SectionSize.GetValueOrDefault(),
                    "archive MD5"
                );

                var otherMD5Section = ReadSection(
                    reader,
                    header.OtherMD5SectionSize.GetValueOrDefault(),
                    "other MD5"
                );

                var signatureSection = ReadSection(
                    reader,
                    header.SignatureSectionSize.GetValueOrDefault(),
                    "signature"
                );

                header = header with
                {
                    FileDataSection = fileDataSection,
                    ArchiveMD5Section = archiveMD5Section,
                    OtherMD5Section = otherMD5Section,
                    SignatureSection = signatureSection,
                };
            }
        }

        return new VPK
        {
            Header = header,
            Files = files,
            Chunks = chunks.ToList(),
        };
    }

    private static byte[] ReadSection(BinaryReader reader, uint size, string name)
    {
        var data = reader.ReadBytes(checked((int)size));

        if (data.Length != size)
        {
            throw new EndOfStreamException($"Unexpected end of VPK {name} section.");
        }

        return data;
    }
}
