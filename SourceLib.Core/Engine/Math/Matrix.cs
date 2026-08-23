namespace SourceLib.Core.Engine.Math;

public sealed class Matrix
{
    public Matrix(IReadOnlyList<float> values)
    {
        if (values.Count != 16)
            throw new ArgumentException("Matrix requires exactly 16 values");

        Values = [.. values];
    }

    public IReadOnlyList<float> Values { get; }
}
