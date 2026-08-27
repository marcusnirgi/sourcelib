using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.DMX;

public sealed class DmxFormatParser : IBinaryFormatParser<DmxDocument>
{
    public DmxDocument Parse(byte[] input)
    {
        var elements = new List<DmxElement>();

        using var stream = new MemoryStream(input.ToArray());
        using var reader = new BinaryReader(stream);

        var headerRaw = BinaryReading.ReadStringUntil(reader, (byte)'\n');
        var header = DmxHeader.FromString(headerRaw);

        if (header.Encoding != "binary")
            throw new InvalidDataException("Expected binary DMX file.");

        var encodingVersion = (DmxHeaderBinaryEncodingVersion)header.EncodingVersion;

        // some quirky headers might have an extra '\n'.
        var next = reader.ReadByte();

        if (next == (byte)'\n')
            next = reader.ReadByte();

        if (next != 0)
            throw new InvalidDataException("Invalid DMX header.");

        var stringMap = ReadStringMap(reader, encodingVersion);

        var elementCount = reader.ReadInt32();

        if (elementCount < 0)
            throw new InvalidDataException("Invalid DMX element count.");

        for (var i = 0; i < elementCount; i++)
        {
            var className = ReadElementType(reader, encodingVersion, stringMap);
            var name = ReadElementName(reader, encodingVersion, stringMap);
            var id = new Guid(reader.ReadBytes(16));

            elements.Add(new DmxElement(className, name, id));
        }

        foreach (var element in elements)
        {
            var attributeCount = reader.ReadInt32();

            if (attributeCount < 0)
                throw new InvalidDataException("Invalid DMX attribute count.");

            var attributes = new List<DmxAttribute>(attributeCount);

            for (var attributeNum = 0; attributeNum < attributeCount; attributeNum++)
            {
                var name = ReadAttributeName(reader, encodingVersion, stringMap);
                var rawTypeIndex = reader.ReadByte();

                var result = ReadAttributeValue(
                    reader,
                    encodingVersion,
                    stringMap,
                    elements,
                    rawTypeIndex
                );

                attributes.Add(
                    new DmxAttribute(
                        name,
                        (DmxTypeIndex)rawTypeIndex,
                        result.Value,
                        result.ReferencedElement
                    )
                );
            }

            element.Attributes = attributes;
        }

        return new DmxDocument(header, elements);
    }

    private static IReadOnlyList<string>? ReadStringMap(
        BinaryReader reader,
        DmxHeaderBinaryEncodingVersion encodingVersion
    )
    {
        int stringCount;

        if (encodingVersion == DmxHeaderBinaryEncodingVersion.V1)
        {
            return null;
        }

        if (
            encodingVersion == DmxHeaderBinaryEncodingVersion.V2
            || encodingVersion == DmxHeaderBinaryEncodingVersion.V3
        )
        {
            stringCount = reader.ReadInt16();
        }
        else if (
            encodingVersion == DmxHeaderBinaryEncodingVersion.V4
            || encodingVersion == DmxHeaderBinaryEncodingVersion.V5
        )
        {
            stringCount = reader.ReadInt32();
        }
        else
        {
            throw new InvalidDataException($"Unknown encoding version {encodingVersion}.");
        }

        if (stringCount < 0)
            throw new InvalidDataException("Invalid DMX string count.");

        var values = new List<string>(stringCount);

        for (var i = 0; i < stringCount; i++)
            values.Add(BinaryReading.ReadStringUntil(reader, 0));

        return values;
    }

    private static string ReadElementType(
        BinaryReader reader,
        DmxHeaderBinaryEncodingVersion encodingVersion,
        IReadOnlyList<string>? stringMap
    )
    {
        if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V2)
            return ReadStringMapValue(reader, encodingVersion, stringMap);

        return BinaryReading.ReadStringUntil(reader, 0);
    }

    private static string ReadElementName(
        BinaryReader reader,
        DmxHeaderBinaryEncodingVersion encodingVersion,
        IReadOnlyList<string>? stringMap
    )
    {
        if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V4)
            return ReadStringMapValue(reader, encodingVersion, stringMap);

        return BinaryReading.ReadStringUntil(reader, 0);
    }

    private static string ReadAttributeName(
        BinaryReader reader,
        DmxHeaderBinaryEncodingVersion encodingVersion,
        IReadOnlyList<string>? stringMap
    )
    {
        if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V2)
            return ReadStringMapValue(reader, encodingVersion, stringMap);

        return BinaryReading.ReadStringUntil(reader, 0);
    }

    private static string ReadScalarString(
        BinaryReader reader,
        DmxHeaderBinaryEncodingVersion encodingVersion,
        IReadOnlyList<string>? stringMap
    )
    {
        if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V4)
            return ReadStringMapValue(reader, encodingVersion, stringMap);

        return BinaryReading.ReadStringUntil(reader, 0);
    }

    private static string ReadStringMapValue(
        BinaryReader reader,
        DmxHeaderBinaryEncodingVersion encodingVersion,
        IReadOnlyList<string>? stringMap
    )
    {
        if (stringMap is null)
            throw new InvalidDataException("DMX string map is not available.");

        int index;

        if (encodingVersion == DmxHeaderBinaryEncodingVersion.V5)
            index = reader.ReadInt32();
        else
            index = reader.ReadInt16();

        if ((uint)index >= (uint)stringMap.Count)
            throw new InvalidDataException($"Invalid string table index {index}.");

        return stringMap[index];
    }

    private static (EngineValue Value, DmxElement? ReferencedElement) ReadAttributeValue(
        BinaryReader reader,
        DmxHeaderBinaryEncodingVersion encodingVersion,
        IReadOnlyList<string>? stringMap,
        List<DmxElement> elements,
        byte rawTypeId
    )
    {
        var type = (DmxTypeIndex)rawTypeId;

        switch (type)
        {
            case DmxTypeIndex.ElementRef:
            {
                var elementIndex = reader.ReadInt32();

                if (elementIndex == -1)
                    return (new EngineGuid(Guid.Empty), null);

                if (elementIndex == -2)
                {
                    var stubId = Guid.Parse(BinaryReading.ReadStringUntil(reader, 0));

                    return (new EngineGuid(stubId), null);
                }

                var element = elements[elementIndex];

                return (new EngineGuid(element.Id), element);
            }

            case DmxTypeIndex.Int:
                return (new EngineInt(reader.ReadInt32()), null);

            case DmxTypeIndex.Float:
                return (new EngineFloat(reader.ReadSingle()), null);

            case DmxTypeIndex.Bool:
                return (new EngineBool(reader.ReadByte() != 0), null);

            case DmxTypeIndex.String:
                return (
                    new EngineString(ReadScalarString(reader, encodingVersion, stringMap)),
                    null
                );

            case DmxTypeIndex.Binary:
            {
                var length = reader.ReadInt32();

                if (length < 0)
                    throw new InvalidDataException("Invalid DMX binary length.");

                return (new EngineBytes(reader.ReadBytes(length)), null);
            }

            case DmxTypeIndex.Time:
            {
                if (encodingVersion < DmxHeaderBinaryEncodingVersion.V3)
                {
                    throw new InvalidDataException(
                        "Time values are not supported by DMX binary versions before v3."
                    );
                }

                return (new EngineTime(reader.ReadInt32() / 10000f), null);
            }

            case DmxTypeIndex.Color:
                return (
                    new EngineColor4(
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte()
                    ),
                    null
                );

            case DmxTypeIndex.Vector2:
                return (new EngineVector2(reader.ReadSingle(), reader.ReadSingle()), null);

            case DmxTypeIndex.Vector3:
                return (
                    new EngineVector3(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    ),
                    null
                );

            case DmxTypeIndex.Vector4:
                return (
                    new EngineVector4(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    ),
                    null
                );

            case DmxTypeIndex.Angle:
                return (
                    new EngineAngle(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    null
                );

            case DmxTypeIndex.Quaternion:
                return (
                    new EngineQuaternion(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    ),
                    null
                );

            case DmxTypeIndex.Matrix:
            {
                var values = new float[16];

                for (var i = 0; i < values.Length; i++)
                    values[i] = reader.ReadSingle();

                return (new EngineMatrix(values), null);
            }

            case DmxTypeIndex.ElementArray:
            {
                var count = reader.ReadInt32();
                var values = new List<EngineGuid>(count);

                for (var i = 0; i < count; i++)
                {
                    var elementIndex = reader.ReadInt32();

                    if (elementIndex == -1)
                    {
                        values.Add(new EngineGuid(Guid.Empty));
                    }
                    else if (elementIndex == -2)
                    {
                        values.Add(
                            new EngineGuid(Guid.Parse(BinaryReading.ReadStringUntil(reader, 0)))
                        );
                    }
                    else
                    {
                        values.Add(new EngineGuid(elements[elementIndex].Id));
                    }
                }

                return (new EngineArray<EngineGuid>(values), null);
            }

            case DmxTypeIndex.IntArray:
                return (ReadArray(reader, r => new EngineInt(r.ReadInt32())), null);

            case DmxTypeIndex.FloatArray:
                return (ReadArray(reader, r => new EngineFloat(r.ReadSingle())), null);

            case DmxTypeIndex.BoolArray:
                return (ReadArray(reader, r => new EngineBool(r.ReadByte() != 0)), null);

            case DmxTypeIndex.StringArray:
            {
                var count = reader.ReadInt32();
                var values = new List<EngineString>(count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(new EngineString(BinaryReading.ReadStringUntil(reader, 0)));
                }

                return (new EngineArray<EngineString>(values), null);
            }

            case DmxTypeIndex.BinaryArray:
            {
                var count = reader.ReadInt32();
                var values = new List<EngineBytes>(count);

                for (var i = 0; i < count; i++)
                {
                    var length = reader.ReadInt32();

                    if (length < 0)
                    {
                        throw new InvalidDataException("Invalid DMX binary array element length.");
                    }

                    values.Add(new EngineBytes(reader.ReadBytes(length)));
                }

                return (new EngineArray<EngineBytes>(values), null);
            }

            case DmxTypeIndex.TimeArray:
            {
                if (encodingVersion < DmxHeaderBinaryEncodingVersion.V3)
                {
                    throw new InvalidDataException(
                        "Time arrays are not supported by DMX binary versions before v3."
                    );
                }

                return (
                    ReadArray(reader, static r => new EngineTime(r.ReadInt32() / 10000f)),
                    null
                );
            }

            case DmxTypeIndex.ColorArray:
                return (
                    ReadArray(
                        reader,
                        static r => new EngineColor4(
                            r.ReadByte(),
                            r.ReadByte(),
                            r.ReadByte(),
                            r.ReadByte()
                        )
                    ),
                    null
                );

            case DmxTypeIndex.Vector2Array:
                return (
                    ReadArray(
                        reader,
                        static r => new EngineVector2(r.ReadSingle(), r.ReadSingle())
                    ),
                    null
                );

            case DmxTypeIndex.Vector3Array:
                return (
                    ReadArray(
                        reader,
                        static r => new EngineVector3(
                            r.ReadSingle(),
                            r.ReadSingle(),
                            r.ReadSingle()
                        )
                    ),
                    null
                );

            case DmxTypeIndex.Vector4Array:
                return (
                    ReadArray(
                        reader,
                        static r => new EngineVector4(
                            r.ReadSingle(),
                            r.ReadSingle(),
                            r.ReadSingle(),
                            r.ReadSingle()
                        )
                    ),
                    null
                );

            case DmxTypeIndex.AngleArray:
                return (
                    ReadArray(
                        reader,
                        static r => new EngineAngle(r.ReadSingle(), r.ReadSingle(), r.ReadSingle())
                    ),
                    null
                );

            case DmxTypeIndex.QuaternionArray:
                return (
                    ReadArray(
                        reader,
                        static r => new EngineQuaternion(
                            r.ReadSingle(),
                            r.ReadSingle(),
                            r.ReadSingle(),
                            r.ReadSingle()
                        )
                    ),
                    null
                );

            case DmxTypeIndex.MatrixArray:
            {
                var count = reader.ReadInt32();
                var values = new List<EngineMatrix>(count);

                for (var i = 0; i < count; i++)
                {
                    var matrix = new float[16];

                    for (var j = 0; j < matrix.Length; j++)
                        matrix[j] = reader.ReadSingle();

                    values.Add(new EngineMatrix(matrix));
                }

                return (new EngineArray<EngineMatrix>(values), null);
            }

            default:
                throw new NotSupportedException($"Unsupported DMX attribute type: {rawTypeId}");
        }
    }

    private static EngineArray<T> ReadArray<T>(BinaryReader reader, Func<BinaryReader, T> readValue)
        where T : EngineValue
    {
        var count = reader.ReadInt32();

        if (count < 0)
            throw new InvalidDataException("Invalid DMX array count.");

        var values = new List<T>(count);

        for (var i = 0; i < count; i++)
            values.Add(readValue(reader));

        return new EngineArray<T>(values);
    }
}
