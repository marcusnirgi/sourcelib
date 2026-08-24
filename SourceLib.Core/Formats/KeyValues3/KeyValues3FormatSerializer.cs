using SourceLib.Core.Engine;

namespace SourceLib.Core.Formats.KeyValues3;

public sealed class KeyValues3FormatSerializer : TextFormatSerializer<KeyValues3Document>
{
    public override void Serialize(KeyValues3Document value, TextWriter writer)
    {
        if (!string.IsNullOrEmpty(value.Header))
            writer.WriteLine(value.Header);

        foreach (var pair in value.Body)
            WritePair(writer, pair, 0);
    }

    private void WritePair(TextWriter writer, KeyValues3Pair pair, int depth)
    {
        WriteIndent(writer, depth);
        WriteQuoted(writer, pair.Key);
        writer.Write(" = ");

        if (pair.Object is not null)
        {
            WriteObject(writer, pair.Object, depth);
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
            WritePair(writer, child, depth + 1);

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
            WriteArrayValue(writer, value, depth + 1);

        WriteIndent(writer, depth);
        writer.Write(']');
    }

    private void WriteArrayValue(TextWriter writer, KeyValues3ArrayValue value, int depth)
    {
        WriteIndent(writer, depth);

        if (value.Children is not null)
        {
            WriteObject(writer, value.Children, depth);
            writer.WriteLine(',');
            return;
        }

        if (value.Array is not null)
        {
            WriteArray(writer, value.Array, depth);
            writer.WriteLine(',');
            return;
        }

        WriteValue(writer, value.Value);
        writer.WriteLine(',');
    }

    private static void WriteValue(TextWriter writer, EngineValue? value)
    {
        switch (value)
        {
            case null:
                writer.Write("null");
                break;

            case EngineString stringValue:
                WriteQuoted(writer, stringValue.Value);
                break;

            case EngineInt intValue:
                writer.Write(
                    intValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                );
                break;

            case EngineFloat floatValue:
                writer.Write(
                    floatValue.Value.ToString(
                        "R",
                        System.Globalization.CultureInfo.InvariantCulture
                    )
                );
                break;

            case EngineBool boolValue:
                writer.Write(boolValue.Value ? "true" : "false");
                break;

            case EngineGuid guidValue:
                WriteQuoted(writer, guidValue.Value.ToString());
                break;

            case EngineBytes bytesValue:
                WriteQuoted(writer, Convert.ToHexString(bytesValue.Value.Span));
                break;

            case EngineTime timeValue:
                writer.Write(
                    Convert.ToString(
                        timeValue.Value,
                        System.Globalization.CultureInfo.InvariantCulture
                    )
                );
                break;

            case EngineColor4 colorValue:
                writer.Write(
                    $"{colorValue.Value.Red} {colorValue.Value.Green} "
                        + $"{colorValue.Value.Blue} {colorValue.Value.Alpha}"
                );
                break;

            case EngineVector2 vectorValue:
                writer.Write(
                    $"{vectorValue.Value.X.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{vectorValue.Value.Y.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}"
                );
                break;

            case EngineVector3 vectorValue:
                writer.Write(
                    $"{vectorValue.Value.X.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{vectorValue.Value.Y.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{vectorValue.Value.Z.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}"
                );
                break;

            case EngineVector4 vectorValue:
                writer.Write(
                    $"{vectorValue.Value.X.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{vectorValue.Value.Y.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{vectorValue.Value.Z.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{vectorValue.Value.W.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}"
                );
                break;

            case EngineAngle angleValue:
                writer.Write(
                    $"{angleValue.Value.Pitch.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{angleValue.Value.Yaw.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{angleValue.Value.Roll.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}"
                );
                break;

            case EngineQuaternion quaternionValue:
                writer.Write(
                    $"{quaternionValue.Value.X.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{quaternionValue.Value.Y.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{quaternionValue.Value.Z.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} "
                        + $"{quaternionValue.Value.W.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}"
                );
                break;

            case EngineMatrix matrixValue:
                for (var i = 0; i < 16; i++)
                {
                    if (i > 0)
                        writer.Write(' ');

                    writer.Write(
                        matrixValue
                            .Value.Values[i]
                            .ToString("R", System.Globalization.CultureInfo.InvariantCulture)
                    );
                }

                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported KV3 value type: {value.GetType().Name}"
                );
        }
    }

    private static void WriteQuoted(TextWriter writer, string value)
    {
        writer.Write('"');
        writer.Write(value.Replace("\"", "\\\""));
        writer.Write('"');
    }

    private static void WriteIndent(TextWriter writer, int depth)
    {
        writer.Write(new string('\t', depth));
    }
}
