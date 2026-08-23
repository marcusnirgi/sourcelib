namespace SourceLib.Core.Engine.Math;

public sealed class Angle
{
    public float Pitch { get; }
    public float Yaw { get; }
    public float Roll { get; }

    public Angle(float pitch, float yaw, float roll)
    {
        Pitch = pitch;
        Yaw = yaw;
        Roll = roll;
    }
}
