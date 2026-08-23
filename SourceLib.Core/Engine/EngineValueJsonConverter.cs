using System.Text.Json;
using System.Text.Json.Serialization;
using SourceLib.Core.Engine;

public sealed class EngineValueJsonConverter : JsonConverter<EngineValue>
{
    public override void Write(
        Utf8JsonWriter writer,
        EngineValue value,
        JsonSerializerOptions options
    )
    {
        switch (value)
        {
            case EngineInt v:
                writer.WriteNumberValue(v.Value);
                break;

            case EngineFloat v:
                writer.WriteNumberValue(v.Value);
                break;

            case EngineBool v:
                writer.WriteBooleanValue(v.Value);
                break;

            case EngineString v:
                writer.WriteStringValue(v.Value);
                break;

            case EngineGuid v:
                writer.WriteStringValue(v.Value);
                break;

            default:
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
                break;
        }
    }

    public override EngineValue Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        throw new NotSupportedException();
    }
}
