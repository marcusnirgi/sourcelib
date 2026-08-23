using SourceLib.Core.Engine;

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

        if (pair.Object != null)
        {
            writer.WriteLine();
            WriteIndent(writer, indent);
            writer.WriteLine("{");

            foreach (var child in pair.Object!)
                SerializePair(child, writer, indent + 1);

            WriteIndent(writer, indent);
            writer.WriteLine("}");
        }
        else
        {
            writer.Write(' ');
            WritePrimitive(writer, pair.Value ?? new EngineString("null"));
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

    private static void WritePrimitive(TextWriter writer, EngineValue value)
    {
        switch (value)
        {
            case EngineString stringValue:
                WriteQuoted(writer, stringValue.Value);
                break;

            case EngineInt intValue:
                writer.Write(intValue.Value);
                break;

            case EngineFloat floatValue:
                writer.Write(PrimitiveFormatter.FormatFloat(floatValue.Value));
                break;
            case EngineBool boolValue:
                writer.Write(boolValue.Value);
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
