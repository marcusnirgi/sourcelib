using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.DMX;

public sealed class DmxFormatSerializer : BinaryFormatSerializer<DmxDocument>
{
    public override byte[] Serialize(DmxDocument value)
    {
        var output = new ArrayBufferWriter<byte>();
        var header = value.Header;
        var encodingVersion = (DmxHeaderBinaryEncodingVersion)header.EncodingVersion;

        var elements = value.Elements.ToList();
        var elementIndices = new Dictionary<Guid, int>();

        for (var i = 0; i < elements.Count; i++)
            elementIndices.Add(elements[i].Id, i);

        var stringMap = BuildStringMap(elements, encodingVersion);
        var stringIndices = new Dictionary<string, int>();

        for (var i = 0; i < stringMap.Count; i++)
            stringIndices.Add(stringMap[i], i);

        WriteHeader(output, header);
        WriteStringMap(output, stringMap, encodingVersion);
        WriteInt32(output, elements.Count);

        foreach (var element in elements)
        {
            if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V2)
                WriteStringIndex(output, stringIndices[element.ClassName], encodingVersion);
            else
                WriteNullString(output, element.ClassName);

            if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V4)
                WriteStringIndex(output, stringIndices[element.Name], encodingVersion);
            else
                WriteNullString(output, element.Name);

            WriteBytes(output, element.Id.ToByteArray());
        }

        foreach (var element in elements)
        {
            var attributes = element.Attributes.ToList();

            WriteInt32(output, attributes.Count);

            foreach (var attribute in attributes)
            {
                if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V2)
                    WriteStringIndex(output, stringIndices[attribute.Key], encodingVersion);
                else
                    WriteNullString(output, attribute.Key);

                switch (attribute.TypeIndex)
                {
                    case DmxTypeIndex.ElementRef:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.ElementRef);
                        WriteElementRef(
                            output,
                            attribute.Value,
                            attribute.ReferencedElement,
                            elementIndices
                        );
                        break;
                    }

                    case DmxTypeIndex.Int:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Int);
                        WriteInt32(output, ((EngineInt)attribute.Value).Value);
                        break;
                    }

                    case DmxTypeIndex.Float:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Float);
                        WriteSingle(output, ((EngineFloat)attribute.Value).Value);
                        break;
                    }

                    case DmxTypeIndex.Bool:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Bool);
                        WriteByte(output, ((EngineBool)attribute.Value).Value ? (byte)1 : (byte)0);
                        break;
                    }

                    case DmxTypeIndex.String:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.String);

                        var stringValue = ((EngineString)attribute.Value).Value;

                        if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V4)
                            WriteStringIndex(output, stringIndices[stringValue], encodingVersion);
                        else
                            WriteNullString(output, stringValue);

                        break;
                    }

                    case DmxTypeIndex.Binary:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Binary);

                        var bytes = ((EngineBytes)attribute.Value).Value.Span;

                        WriteInt32(output, bytes.Length);
                        WriteBytes(output, bytes);

                        break;
                    }

                    case DmxTypeIndex.Time:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Time);
                        WriteInt32(
                            output,
                            checked((int)(((EngineTime)attribute.Value).Value.Seconds * 10000f))
                        );
                        break;
                    }

                    case DmxTypeIndex.Color:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Color);

                        var color = ((EngineColor4)attribute.Value).Value;

                        WriteByte(output, checked((byte)color.Red));
                        WriteByte(output, checked((byte)color.Green));
                        WriteByte(output, checked((byte)color.Blue));
                        WriteByte(output, checked((byte)color.Alpha));

                        break;
                    }

                    case DmxTypeIndex.Vector2:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Vector2);

                        var vector = ((EngineVector2)attribute.Value).Value;

                        WriteSingle(output, vector.X);
                        WriteSingle(output, vector.Y);

                        break;
                    }

                    case DmxTypeIndex.Vector3:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Vector3);

                        var vector = ((EngineVector3)attribute.Value).Value;

                        WriteSingle(output, vector.X);
                        WriteSingle(output, vector.Y);
                        WriteSingle(output, vector.Z);

                        break;
                    }

                    case DmxTypeIndex.Vector4:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Vector4);

                        var vector = ((EngineVector4)attribute.Value).Value;

                        WriteSingle(output, vector.X);
                        WriteSingle(output, vector.Y);
                        WriteSingle(output, vector.Z);
                        WriteSingle(output, vector.W);

                        break;
                    }

                    case DmxTypeIndex.Angle:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Angle);

                        var angle = ((EngineAngle)attribute.Value).Value;

                        WriteSingle(output, angle.Pitch);
                        WriteSingle(output, angle.Yaw);
                        WriteSingle(output, angle.Roll);

                        break;
                    }

                    case DmxTypeIndex.Quaternion:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Quaternion);

                        var quaternion = ((EngineQuaternion)attribute.Value).Value;

                        WriteSingle(output, quaternion.X);
                        WriteSingle(output, quaternion.Y);
                        WriteSingle(output, quaternion.Z);
                        WriteSingle(output, quaternion.W);

                        break;
                    }

                    case DmxTypeIndex.Matrix:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Matrix);

                        var values = ((EngineMatrix)attribute.Value).Value.Values;

                        for (var i = 0; i < 16; i++)
                            WriteSingle(output, values[i]);

                        break;
                    }

                    case DmxTypeIndex.ElementArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.ElementArray);

                        var values = ((EngineArray<EngineGuid>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var valueGuid in values)
                            WriteElementRef(output, valueGuid, null, elementIndices);

                        break;
                    }

                    case DmxTypeIndex.IntArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.IntArray);

                        var values = ((EngineArray<EngineInt>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                            WriteInt32(output, item.Value);

                        break;
                    }

                    case DmxTypeIndex.FloatArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.FloatArray);

                        var values = ((EngineArray<EngineFloat>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                            WriteSingle(output, item.Value);

                        break;
                    }

                    case DmxTypeIndex.BoolArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.BoolArray);

                        var values = ((EngineArray<EngineBool>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                            WriteByte(output, item.Value ? (byte)1 : (byte)0);

                        break;
                    }

                    case DmxTypeIndex.StringArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.StringArray);

                        var values = ((EngineArray<EngineString>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                            WriteNullString(output, item.Value);

                        break;
                    }

                    case DmxTypeIndex.BinaryArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.BinaryArray);

                        var values = ((EngineArray<EngineBytes>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            var bytes = item.Value.Span;

                            WriteInt32(output, bytes.Length);
                            WriteBytes(output, bytes);
                        }

                        break;
                    }

                    case DmxTypeIndex.TimeArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.TimeArray);

                        var values = ((EngineArray<EngineTime>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            WriteInt32(output, checked((int)(item.Value.Seconds * 10000f)));
                        }

                        break;
                    }

                    case DmxTypeIndex.ColorArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.ColorArray);

                        var values = ((EngineArray<EngineColor4>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            WriteByte(output, checked((byte)item.Value.Red));
                            WriteByte(output, checked((byte)item.Value.Green));
                            WriteByte(output, checked((byte)item.Value.Blue));
                            WriteByte(output, checked((byte)item.Value.Alpha));
                        }

                        break;
                    }

                    case DmxTypeIndex.Vector2Array:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Vector2Array);

                        var values = ((EngineArray<EngineVector2>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            WriteSingle(output, item.Value.X);
                            WriteSingle(output, item.Value.Y);
                        }

                        break;
                    }

                    case DmxTypeIndex.Vector3Array:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Vector3Array);

                        var values = ((EngineArray<EngineVector3>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            WriteSingle(output, item.Value.X);
                            WriteSingle(output, item.Value.Y);
                            WriteSingle(output, item.Value.Z);
                        }

                        break;
                    }

                    case DmxTypeIndex.Vector4Array:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.Vector4Array);

                        var values = ((EngineArray<EngineVector4>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            WriteSingle(output, item.Value.X);
                            WriteSingle(output, item.Value.Y);
                            WriteSingle(output, item.Value.Z);
                            WriteSingle(output, item.Value.W);
                        }

                        break;
                    }

                    case DmxTypeIndex.AngleArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.AngleArray);

                        var values = ((EngineArray<EngineAngle>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            WriteSingle(output, item.Value.Pitch);
                            WriteSingle(output, item.Value.Yaw);
                            WriteSingle(output, item.Value.Roll);
                        }

                        break;
                    }

                    case DmxTypeIndex.QuaternionArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.QuaternionArray);

                        var values = ((EngineArray<EngineQuaternion>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            WriteSingle(output, item.Value.X);
                            WriteSingle(output, item.Value.Y);
                            WriteSingle(output, item.Value.Z);
                            WriteSingle(output, item.Value.W);
                        }

                        break;
                    }

                    case DmxTypeIndex.MatrixArray:
                    {
                        WriteByte(output, (byte)DmxTypeIndex.MatrixArray);

                        var values = ((EngineArray<EngineMatrix>)attribute.Value).Values;

                        WriteInt32(output, values.Count);

                        foreach (var item in values)
                        {
                            var matrix = item.Value.Values;

                            for (var i = 0; i < 16; i++)
                                WriteSingle(output, matrix[i]);
                        }

                        break;
                    }

                    default:
                        throw new NotSupportedException(
                            $"Unsupported DMX type {attribute.TypeIndex}."
                        );
                }
            }
        }

        return output.WrittenMemory.ToArray();
    }

    private static List<string> BuildStringMap(
        IReadOnlyList<DmxElement> elements,
        DmxHeaderBinaryEncodingVersion encodingVersion
    )
    {
        if (encodingVersion == DmxHeaderBinaryEncodingVersion.V1)
            return [];

        var values = new HashSet<string> { "name" };

        foreach (var element in elements)
        {
            values.Add(element.ClassName);

            if (encodingVersion >= DmxHeaderBinaryEncodingVersion.V4)
                values.Add(element.Name);

            foreach (var attribute in element.Attributes)
            {
                values.Add(attribute.Key);

                if (
                    encodingVersion >= DmxHeaderBinaryEncodingVersion.V4
                    && attribute.TypeIndex == DmxTypeIndex.String
                )
                {
                    values.Add(((EngineString)attribute.Value).Value);
                }
            }
        }

        return values.OrderBy(value => value, StringComparer.Ordinal).ToList();
    }

    private static void WriteHeader(IBufferWriter<byte> output, DmxHeader header)
    {
        var text =
            $"<!-- dmx encoding binary {header.EncodingVersion} "
            + $"format {header.Format} {header.FormatVersion} -->\n\0";

        WriteBytes(output, Encoding.ASCII.GetBytes(text));
    }

    private static void WriteStringMap(
        IBufferWriter<byte> output,
        IReadOnlyList<string> values,
        DmxHeaderBinaryEncodingVersion encodingVersion
    )
    {
        if (encodingVersion == DmxHeaderBinaryEncodingVersion.V1)
            return;

        if (
            encodingVersion == DmxHeaderBinaryEncodingVersion.V2
            || encodingVersion == DmxHeaderBinaryEncodingVersion.V3
        )
            WriteInt16(output, checked((short)values.Count));
        else
            WriteInt32(output, values.Count);

        foreach (var value in values)
            WriteNullString(output, value);
    }

    private static void WriteStringIndex(
        IBufferWriter<byte> output,
        int index,
        DmxHeaderBinaryEncodingVersion encodingVersion
    )
    {
        if (encodingVersion == DmxHeaderBinaryEncodingVersion.V5)
            WriteInt32(output, index);
        else
            WriteInt16(output, checked((short)index));
    }

    private static void WriteElementRef(
        IBufferWriter<byte> output,
        EngineValue value,
        DmxElement? referencedElement,
        IReadOnlyDictionary<Guid, int> elementIndices
    )
    {
        var guid = ((EngineGuid)value).Value;

        if (guid == Guid.Empty)
        {
            WriteInt32(output, -1);
            return;
        }

        if (
            referencedElement is not null
            && elementIndices.TryGetValue(referencedElement.Id, out var referencedIndex)
        )
        {
            WriteInt32(output, referencedIndex);
            return;
        }

        if (elementIndices.TryGetValue(guid, out var index))
        {
            WriteInt32(output, index);
            return;
        }

        WriteInt32(output, -2);
        WriteNullString(output, guid.ToString());
    }

    private static void WriteNullString(IBufferWriter<byte> output, string value)
    {
        WriteBytes(output, Encoding.ASCII.GetBytes(value));
        WriteByte(output, 0);
    }

    private static void WriteByte(IBufferWriter<byte> output, byte value)
    {
        output.GetSpan(1)[0] = value;
        output.Advance(1);
    }

    private static void WriteBytes(IBufferWriter<byte> output, ReadOnlySpan<byte> value)
    {
        if (value.IsEmpty)
            return;

        value.CopyTo(output.GetSpan(value.Length));
        output.Advance(value.Length);
    }

    private static void WriteInt16(IBufferWriter<byte> output, short value)
    {
        BinaryPrimitives.WriteInt16LittleEndian(output.GetSpan(2), value);
        output.Advance(2);
    }

    private static void WriteInt32(IBufferWriter<byte> output, int value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(output.GetSpan(4), value);
        output.Advance(4);
    }

    private static void WriteSingle(IBufferWriter<byte> output, float value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(
            output.GetSpan(4),
            BitConverter.SingleToInt32Bits(value)
        );

        output.Advance(4);
    }
}
