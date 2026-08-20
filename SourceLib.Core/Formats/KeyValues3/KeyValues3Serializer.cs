namespace SourceLib.Core.Formats.KeyValues3;

public sealed class KeyValues3FormatSerializer : ITextFormatSerializer<KeyValues3Document>
{
    public void Serialize(KeyValues3Document value, TextWriter writer)
    {
        if (!string.IsNullOrEmpty(value.Header))
        {
            writer.WriteLine(value.Header);
        }

        foreach (var pair in value.Body)
        {
            WritePair(writer, pair, 0);
        }
    }

    private void WritePair(TextWriter writer, KeyValues3Pair pair, int depth)
    {
        WriteIndent(writer, depth);
        WriteQuoted(writer, pair.Key);
        writer.Write(" = ");

        if (pair.Children is not null)
        {
            WriteObject(writer, pair.Children, depth);
            writer.WriteLine();
            return;
        }

        if (pair.Array is not null)
        {
            WriteArray(writer, pair.Array, depth);
            writer.WriteLine();
            return;
        }

        WriteValue(writer, pair.Value);
        writer.WriteLine();
    }

    private void WriteObject(TextWriter writer, IReadOnlyList<KeyValues3Pair> children, int depth)
    {
        writer.WriteLine("{");

        foreach (var child in children)
        {
            WritePair(writer, child, depth + 1);
        }

        WriteIndent(writer, depth);
        writer.Write('}');
    }

    private void WriteArray(
        TextWriter writer,
        IReadOnlyList<KeyValues3ArrayValue> values,
        int depth
    )
    {
        writer.WriteLine("[");

        foreach (var value in values)
        {
            WriteArrayValue(writer, value, depth + 1);
        }

        WriteIndent(writer, depth);
        writer.Write(']');
    }

    private void WriteArrayValue(TextWriter writer, KeyValues3ArrayValue value, int depth)
    {
        WriteIndent(writer, depth);

        if (value.Children is not null)
        {
            WriteObject(writer, value.Children, depth);
            writer.WriteLine();
            return;
        }

        if (value.Array is not null)
        {
            WriteArray(writer, value.Array, depth);
            writer.WriteLine();
            return;
        }

        WriteValue(writer, value.Value);
        writer.WriteLine(',');
    }

    private void WriteValue(TextWriter writer, KeyValueValue value)
    {
        var primitive =
            value.Primitive
            ?? throw new InvalidOperationException("KeyValues3 value has no primitive value.");

        switch (primitive.Type)
        {
            case ValuePrimitiveType.String:
                WriteQuoted(writer, primitive.String ?? string.Empty);
                break;

            case ValuePrimitiveType.Integer:
                writer.Write(
                    primitive.Integer.ToString(System.Globalization.CultureInfo.InvariantCulture)
                );
                break;

            case ValuePrimitiveType.Float:
                writer.Write(ValuePrimitiveFormatter.FormatFloat(primitive.Float));
                break;

            case ValuePrimitiveType.Boolean:
                writer.Write(primitive.Boolean ? "true" : "false");
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported KV3 primitive type: {primitive.Type}"
                );
        }
    }

    private static void WriteQuoted(TextWriter writer, string value)
    {
        writer.Write('"');
        writer.Write(value.Replace("\\", "\\\\").Replace("\"", "\\\""));
        writer.Write('"');
    }

    private static void WriteIndent(TextWriter writer, int depth)
    {
        writer.Write(new string('\t', depth));
    }
}
