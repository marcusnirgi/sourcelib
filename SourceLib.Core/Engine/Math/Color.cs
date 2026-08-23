namespace SourceLib.Core.Engine.Math;

public sealed class Color3
{
    public float Red { get; }
    public float Green { get; }
    public float Blue { get; }

    public Color3(float r, float g, float b)
    {
        Red = r;
        Green = g;
        Blue = b;
    }
}

public sealed class Color4
{
    public float Red { get; }
    public float Green { get; }
    public float Blue { get; }
    public float Alpha { get; }

    public Color4(float r, float g, float b, float alpha)
    {
        Red = r;
        Green = g;
        Blue = b;
        Alpha = alpha;
    }
}
