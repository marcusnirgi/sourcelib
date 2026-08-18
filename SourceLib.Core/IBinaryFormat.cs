using System.Buffers;

namespace SourceLib.Core;

public interface IBinaryFormatParser<T>
{
    T Parse(ReadOnlySpan<byte> input);
}

public interface IBinaryFormatSerializer<T>
{
    void Serialize(T value, IBufferWriter<byte> output);
}
