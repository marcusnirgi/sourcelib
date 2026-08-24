namespace SourceLib.Core;

public abstract class TextFormatSerializer<T> : ITextFormatSerializer<T>
{
    public abstract void Serialize(T value, TextWriter writer);

    public string SerializeToString(T value)
    {
        var writer = new StringWriter();
        Serialize(value, writer);
        return writer.ToString();
    }
}
