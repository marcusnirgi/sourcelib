namespace SourceLib.Core.Formats.KeyValues2;

public sealed class KeyValues2FormatSerializer : ITextFormatSerializer<KeyValues2Document>
{
    public void Serialize(KeyValues2Document value, TextWriter writer)
    {
        if (value.Header is not null)
        {
            writer.WriteLine(value.Header);
        }

        foreach (var pair in value.Body)
        {
            WritePair(writer, pair, 0);
        }
    }

    private void WritePair(TextWriter writer, KeyValues2Pair pair, int depth)
    {
        WriteIndent(writer, depth);
        WriteQuoted(writer, pair.Key);

        if (pair.Array is not null)
        {
            writer.Write(' ');
            WriteQuoted(writer, pair.TypeHint!);
            writer.WriteLine();

            WriteIndent(writer, depth);
            writer.WriteLine("[");

            foreach (var element in pair.Array)
            {
                WriteArrayValue(writer, element, depth + 1);
            }

            WriteIndent(writer, depth);
            writer.WriteLine("]");
            return;
        }

        if (pair.Children is not null)
        {
            if (pair.TypeHint is not null)
            {
                writer.Write(' ');
                WriteQuoted(writer, pair.TypeHint);
            }

            writer.WriteLine();

            WriteIndent(writer, depth);
            writer.WriteLine("{");

            foreach (var child in pair.Children)
            {
                WritePair(writer, child, depth + 1);
            }

            WriteIndent(writer, depth);
            writer.WriteLine("}");
            return;
        }

        writer.Write(' ');
        WriteQuoted(writer, pair.TypeHint!);
        writer.Write(' ');
        WritePrimitive(writer, pair.Value);
        writer.WriteLine();
    }

    private void WritePrimitive(TextWriter writer, KeyValueValue value)
    {
        var primitive =
            value.Primitive
            ?? throw new InvalidOperationException("KeyValues2 value has no primitive value.");

        var serialized = primitive.Type switch
        {
            ValuePrimitiveType.String => primitive.String,
            ValuePrimitiveType.Integer => primitive.Integer.ToString(),
            ValuePrimitiveType.Float => primitive.Float.ToString(),
            ValuePrimitiveType.Boolean => primitive.Boolean ? "1" : "0",
            _ => throw new InvalidOperationException(
                $"Unsupported primitive type '{primitive.Type}'."
            ),
        };

        WriteQuoted(writer, serialized ?? string.Empty);
    }

    private void WriteArrayValue(TextWriter writer, KeyValues2ArrayValue value, int depth)
    {
        WriteIndent(writer, depth);

        if (value.Children is not null)
        {
            if (value.TypeHint is not null)
            {
                WriteQuoted(writer, value.TypeHint);
                writer.WriteLine();
                WriteIndent(writer, depth);
            }

            writer.WriteLine("{");

            foreach (var child in value.Children)
            {
                WritePair(writer, child, depth + 1);
            }

            WriteIndent(writer, depth);
            writer.WriteLine("}");
            return;
        }

        if (value.TypeHint is not null)
        {
            WriteQuoted(writer, value.TypeHint);
            writer.Write(' ');
        }

        WritePrimitive(writer, value.Value);
        writer.WriteLine();
    }

    private void WriteIndent(TextWriter writer, int depth)
    {
        writer.Write(new string('\t', depth));
    }

    private void WriteQuoted(TextWriter writer, string value)
    {
        writer.Write('"');
        writer.Write(value.Replace("\"", "\\\""));
        writer.Write('"');
    }
}
