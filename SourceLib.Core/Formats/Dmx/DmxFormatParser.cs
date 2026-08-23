using System.Collections.Immutable;
using System.Linq.Expressions;
using SourceLib.Core.Engine;
using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.Dmx;

public sealed class DmxFormatParser : IBinaryFormatParser<DmxDocument>
{
    public DmxDocument Parse(ReadOnlySpan<byte> input)
    {
        var elements = new List<DmxElement>();

        var stream = new MemoryStream(input.ToArray());
        using var reader = new BinaryReader(stream);

        var header = BinaryHelper.ReadStringUntil(reader, (byte)'\n');

        // some quirky headers might have double \n.
        var next = reader.ReadByte();

        if (next == (byte)'\n')
            next = reader.ReadByte();

        if (next != 0)
            throw new InvalidDataException("Invalid DMX header.");

        var elementCount = reader.ReadUInt32();

        for (var i = 0; i < elementCount; i++)
        {
            var className = BinaryHelper.ReadStringUntil(reader, 0);
            var name = BinaryHelper.ReadStringUntil(reader, 0);
            var id = new Guid(reader.ReadBytes(16));

            elements.Add(new DmxElement(className, name, id));
        }

        foreach (var element in elements)
        {
            var attributes = new List<DmxAttribute>();
            var attributeCount = reader.ReadUInt32();

            for (var attributeNum = 0; attributeNum < attributeCount; attributeNum++)
            {
                var name = BinaryHelper.ReadStringUntil(reader, 0);
                var rawTypeIndex = reader.ReadByte();
                var value = ReadAttributeValue(reader, elements, rawTypeIndex);

                attributes.Add(new DmxAttribute(name, (DmxTypeIndex)rawTypeIndex, value));
            }

            element.Attributes = attributes;
        }

        return new DmxDocument(header, elements);
    }

    private static EngineValue ReadAttributeValue(
        BinaryReader reader,
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
                var element = elements[elementIndex];

                return new EngineGuid(element.Id);
            }

            case DmxTypeIndex.Int:
                return new EngineInt(reader.ReadInt32());

            case DmxTypeIndex.Float:
                return new EngineFloat(reader.ReadSingle());

            case DmxTypeIndex.Bool:
                return new EngineBool(reader.ReadBoolean());

            case DmxTypeIndex.String:
                return new EngineString(BinaryHelper.ReadStringUntil(reader, 0));

            case DmxTypeIndex.Binary:
                return new EngineBytes(BinaryHelper.ReadBytesUntil(reader, 0));

            case DmxTypeIndex.Color:
                return new EngineColor4(
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte()
                );

            case DmxTypeIndex.ElementArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineGuid>((int)count);

                for (var i = 0; i < count; i++)
                {
                    var elementIndex = reader.ReadInt32();
                    values.Add(new EngineGuid(elements[elementIndex].Id));
                }

                return new EngineArray<EngineGuid>(values);
            }

            case DmxTypeIndex.IntArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineInt>((int)count);

                for (var i = 0; i < count; i++)
                    values.Add(new EngineInt(reader.ReadInt32()));

                return new EngineArray<EngineInt>(values);
            }

            case DmxTypeIndex.FloatArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineFloat>((int)count);

                for (var i = 0; i < count; i++)
                    values.Add(new EngineFloat(reader.ReadSingle()));

                return new EngineArray<EngineFloat>(values);
            }

            case DmxTypeIndex.BoolArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineBool>((int)count);

                for (var i = 0; i < count; i++)
                    values.Add(new EngineBool(reader.ReadBoolean()));

                return new EngineArray<EngineBool>(values);
            }

            case DmxTypeIndex.StringArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineString>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(new EngineString(BinaryHelper.ReadStringUntil(reader, 0)));
                }

                return new EngineArray<EngineString>(values);
            }

            case DmxTypeIndex.BinaryArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<ReadOnlyMemory<byte>>((int)count);

                for (var i = 0; i < count; i++)
                    values.Add(BinaryHelper.ReadBytesUntil(reader, 0));

                return new EngineByteArray(values);
            }

            case DmxTypeIndex.Vector2:
            {
                return new EngineVector2(reader.ReadSingle(), reader.ReadSingle());
            }

            case DmxTypeIndex.Vector3:
            {
                return new EngineVector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
            }

            case DmxTypeIndex.Vector4:
            {
                return new EngineVector4(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
            }

            case DmxTypeIndex.Angle:
            {
                return new EngineAngle(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
            }

            case DmxTypeIndex.Vector2Array:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineVector2>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(new EngineVector2(reader.ReadSingle(), reader.ReadSingle()));
                }

                return new EngineArray<EngineVector2>(values);
            }

            case DmxTypeIndex.Vector3Array:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineVector3>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(
                        new EngineVector3(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle()
                        )
                    );
                }

                return new EngineArray<EngineVector3>(values);
            }

            case DmxTypeIndex.Vector4Array:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineVector4>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(
                        new EngineVector4(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle()
                        )
                    );
                }

                return new EngineArray<EngineVector4>(values);
            }

            case DmxTypeIndex.AngleArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineAngle>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(
                        new EngineAngle(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle()
                        )
                    );
                }

                return new EngineArray<EngineAngle>(values);
            }

            case DmxTypeIndex.QuaternionArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineQuaternion>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(
                        new EngineQuaternion(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle()
                        )
                    );
                }

                return new EngineArray<EngineQuaternion>(values);
            }

            case DmxTypeIndex.ColorArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineColor4>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(
                        new EngineColor4(
                            reader.ReadByte(),
                            reader.ReadByte(),
                            reader.ReadByte(),
                            reader.ReadByte()
                        )
                    );
                }

                return new EngineArray<EngineColor4>(values);
            }

            case DmxTypeIndex.Quaternion:
            {
                return new EngineQuaternion(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );
            }

            case DmxTypeIndex.Time:
            {
                return new EngineTime(reader.ReadInt32() / 10000f);
            }

            case DmxTypeIndex.TimeArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineTime>((int)count);

                for (var i = 0; i < count; i++)
                {
                    values.Add(new EngineTime(reader.ReadInt32() / 10000f));
                }

                return new EngineArray<EngineTime>(values);
            }

            case DmxTypeIndex.Matrix:
            {
                var values = new float[16];

                for (var i = 0; i < 16; i++)
                    values[i] = reader.ReadSingle();

                return new EngineMatrix(values);
            }

            case DmxTypeIndex.MatrixArray:
            {
                var count = reader.ReadUInt32();
                var values = new List<EngineMatrix>((int)count);

                for (var i = 0; i < count; i++)
                {
                    var matrix = new float[16];

                    for (var j = 0; j < 16; j++)
                        matrix[j] = reader.ReadSingle();

                    values.Add(new EngineMatrix(matrix));
                }

                return new EngineArray<EngineMatrix>(values);
            }

            default:
                throw new NotSupportedException($"Unsupported DMX attribute type: {rawTypeId}");
        }
    }
}
