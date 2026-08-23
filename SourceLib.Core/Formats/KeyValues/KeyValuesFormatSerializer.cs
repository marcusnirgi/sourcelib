namespace SourceLib.Core.Formats.KeyValues;

public sealed class KeyValues1FormatSerializer : ITextFormatSerializer<KeyValuesDocument>
{
    public void Serialize(KeyValuesDocument document, TextWriter writer)
    {
        foreach (var macro in document.Macros)
        {
            writer.Write('#');
            writer.WriteLine(macro);
        }

        foreach (var pair in document.Body)
            SerializePair(pair, writer, 0);
    }

    private void SerializePair(KeyValuesPair pair, TextWriter writer, int indent)
    {
        WriteIndent(writer, indent);
        WriteQuoted(writer, pair.Key);
        WriteTags(writer, pair.Tags);

        if (pair.Children != null)
        {
            writer.WriteLine();
            WriteIndent(writer, indent);
            writer.WriteLine("{");

            foreach (var child in pair.Children!)
                SerializePair(child, writer, indent + 1);

            WriteIndent(writer, indent);
            writer.WriteLine("}");
        }
        else
        {
            writer.Write(' ');
            WritePrimitive(writer, pair.Value);
            writer.WriteLine();
        }
    }

    private static void WriteTags(TextWriter writer, IReadOnlyList<string> tags)
    {
        foreach (var tag in tags)
        {
            writer.Write(" [");
            writer.Write(tag);
            writer.Write(']');
        }
    }

    private static void WritePrimitive(TextWriter writer, ValuePrimitive value)
    {
        switch (value.Type)
        {
            case ValuePrimitiveType.String:
                WriteQuoted(writer, value.String!);
                break;

            case ValuePrimitiveType.Integer:
                writer.Write(value.Integer);
                break;

            case ValuePrimitiveType.Float:
                writer.Write(ValuePrimitiveFormatter.FormatFloat(value.Float));
                break;
            case ValuePrimitiveType.Boolean:
                writer.Write(value.Boolean);
                break;
        }
    }

    private static void WriteQuoted(TextWriter writer, string value)
    {
        writer.Write('"');
        writer.Write(value);
        writer.Write('"');
    }

    private static void WriteIndent(TextWriter writer, int level)
    {
        writer.Write(new string('\t', level));
    }
}
