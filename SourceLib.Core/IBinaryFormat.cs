namespace SourceLib.Core;

public interface IBinaryFormatParser<T>
{
    T Parse(byte[] input);
}

public interface IBinaryFormatSerializer<T>
{
    byte[] Serialize(T value);
}
