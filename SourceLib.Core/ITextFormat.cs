namespace SourceLib.Core;

public interface ITextFormatParser<T>
{
    T Parse(ReadOnlySpan<char> input);
}

public interface ITextFormatSerializer<T>
{
    void Serialize(T value, TextWriter writer);
}
