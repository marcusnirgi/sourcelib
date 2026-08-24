namespace SourceLib.Core;

public abstract class BinaryFormatSerializer<T> : IBinaryFormatSerializer<T>
{
    public abstract byte[] Serialize(T value);
}
