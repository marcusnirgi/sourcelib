using System.Globalization;
using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.KeyValues2;

public sealed class KeyValues2FormatSerializer : ITextFormatSerializer<KeyValues2Document>
{
    public void Serialize(KeyValues2Document value, TextWriter writer)
    {
        if (value.Header is not null)
            writer.WriteLine(value.Header);

        foreach (var pair in value.Body)
            WritePair(writer, pair, 0);
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

            for (var i = 0; i < pair.Array.Count; i++)
                WriteArrayItem(writer, pair.Array[i], depth + 1, i < pair.Array.Count - 1);

            WriteIndent(writer, depth);
            writer.WriteLine("]");
            return;
        }

        if (pair.Object is not null)
        {
            if (pair.TypeHint is not null)
            {
                writer.Write(' ');
                WriteQuoted(writer, pair.TypeHint);
            }

            writer.WriteLine();

            WriteIndent(writer, depth);
            writer.WriteLine("{");

            foreach (var child in pair.Object)
                WritePair(writer, child, depth + 1);

            WriteIndent(writer, depth);
            writer.WriteLine("}");
            return;
        }

        writer.Write(' ');
        WriteQuoted(writer, pair.TypeHint!);
        writer.Write(' ');
        WriteValue(writer, pair.Value);
        writer.WriteLine();
    }

    private void WriteArrayItem(TextWriter writer, KeyValues2ArrayItem item, int depth, bool comma)
    {
        WriteIndent(writer, depth);

        if (item.Array is not null)
        {
            if (item.TypeHint is not null)
            {
                WriteQuoted(writer, item.TypeHint);
                writer.WriteLine();
                WriteIndent(writer, depth);
            }

            writer.WriteLine("[");

            for (var i = 0; i < item.Array.Count; i++)
                WriteArrayItem(writer, item.Array[i], depth + 1, i < item.Array.Count - 1);

            WriteIndent(writer, depth);
            writer.Write(']');

            if (comma)
                writer.Write(',');

            writer.WriteLine();
            return;
        }

        if (item.Children is not null)
        {
            if (item.TypeHint is not null)
            {
                WriteQuoted(writer, item.TypeHint);
                writer.WriteLine();
                WriteIndent(writer, depth);
            }

            writer.WriteLine("{");

            foreach (var child in item.Children)
                WritePair(writer, child, depth + 1);

            WriteIndent(writer, depth);
            writer.Write('}');

            if (comma)
                writer.Write(',');

            writer.WriteLine();
            return;
        }

        if (item.TypeHint is not null)
        {
            WriteQuoted(writer, item.TypeHint);
            writer.Write(' ');
        }

        WriteValue(writer, item.Value);

        if (comma)
            writer.Write(',');

        writer.WriteLine();
    }

    private static void WriteValue(TextWriter writer, EngineValue? value)
    {
        switch (value)
        {
            case null:
                WriteQuoted(writer, string.Empty);
                break;

            case EngineBool boolValue:
                WriteQuoted(writer, boolValue.Value ? "1" : "0");
                break;

            case EngineInt intValue:
                WriteQuoted(writer, intValue.Value.ToString());
                break;

            case EngineFloat floatValue:
                WriteQuoted(writer, PrimitiveFormatter.FormatFloat(floatValue.Value));
                break;

            case EngineString stringValue:
                WriteQuoted(writer, stringValue.Value);
                break;

            case EngineGuid guidValue:
                WriteQuoted(writer, guidValue.Value.ToString());
                break;

            case EngineBytes bytesValue:
                WriteQuoted(writer, Convert.ToHexString(bytesValue.Value.Span));
                break;

            case EngineTime timeValue:
                WriteQuoted(
                    writer,
                    timeValue.Value.Seconds.ToString("R", CultureInfo.InvariantCulture)
                );
                break;

            case EngineColor4 colorValue:
                WriteQuoted(
                    writer,
                    $"{colorValue.Value.Red} {colorValue.Value.Green} "
                        + $"{colorValue.Value.Blue} {colorValue.Value.Alpha}"
                );
                break;

            case EngineVector2 vectorValue:
                WriteQuoted(writer, $"{vectorValue.Value.X} {vectorValue.Value.Y}");
                break;

            case EngineVector3 vectorValue:
                WriteQuoted(
                    writer,
                    $"{vectorValue.Value.X} {vectorValue.Value.Y} " + $"{vectorValue.Value.Z}"
                );
                break;

            case EngineVector4 vectorValue:
                WriteQuoted(
                    writer,
                    $"{vectorValue.Value.X} {vectorValue.Value.Y} "
                        + $"{vectorValue.Value.Z} {vectorValue.Value.W}"
                );
                break;

            case EngineAngle angleValue:
                WriteQuoted(
                    writer,
                    $"{angleValue.Value.Pitch} {angleValue.Value.Yaw} " + $"{angleValue.Value.Roll}"
                );
                break;

            case EngineQuaternion quaternionValue:
                WriteQuoted(
                    writer,
                    $"{quaternionValue.Value.X} {quaternionValue.Value.Y} "
                        + $"{quaternionValue.Value.Z} {quaternionValue.Value.W}"
                );
                break;

            case EngineMatrix matrixValue:
                WriteQuoted(writer, string.Join(' ', matrixValue.Value));
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported primitive type '{value.GetType().Name}'."
                );
        }
    }

    private static void WriteIndent(TextWriter writer, int depth)
    {
        writer.Write(new string('\t', depth));
    }

    private static void WriteQuoted(TextWriter writer, string value)
    {
        writer.Write('"');
        writer.Write(value.Replace("\"", "\\\""));
        writer.Write('"');
    }
}
