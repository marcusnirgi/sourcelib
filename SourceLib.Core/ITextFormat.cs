namespace SourceLib.Core;

public interface ITextFormatParser<T>
{
    T Parse(string input);
}

public interface ITextFormatSerializer<T>
{
    void Serialize(T value, TextWriter writer);
}
