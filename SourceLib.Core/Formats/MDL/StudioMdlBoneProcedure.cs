using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public interface IStudioMdlBoneProcedure { }

public sealed class StudioMdlAxisInterpBone : IStudioMdlBoneProcedure
{
    public required int Control { get; set; }

    public required int Axis { get; set; }

    public required Vector3[] Position { get; set; }

    public required Quaternion[] Quaternion { get; set; }
}

public sealed class StudioMdlQuatInterpInfo
{
    public required float InverseTolerance { get; set; }

    public required Quaternion Trigger { get; set; }

    public required Vector3 Position { get; set; }

    public required Quaternion Quaternion { get; set; }
}

public sealed class StudioMdlQuatInterpBone : IStudioMdlBoneProcedure
{
    public required int Control { get; set; }

    public required int TriggerCount { get; set; }

    public required int TriggerIndex { get; set; }

    public required StudioMdlQuatInterpInfo[] Triggers { get; set; }
}

public sealed class StudioMdlAimAtBone : IStudioMdlBoneProcedure
{
    public required int Parent { get; set; }

    public required int Aim { get; set; }

    public required Vector3 AimVector { get; set; }

    public required Vector3 UpVector { get; set; }

    public required Vector3 BasePosition { get; set; }
}

public sealed class StudioMdlJiggleBone : IStudioMdlBoneProcedure
{
    public required int Flags { get; set; }

    public required float Length { get; set; }

    public required float TipMass { get; set; }

    public required float YawStiffness { get; set; }

    public required float YawDamping { get; set; }

    public required float PitchStiffness { get; set; }

    public required float PitchDamping { get; set; }

    public required float AlongStiffness { get; set; }

    public required float AlongDamping { get; set; }

    public required float AngleLimit { get; set; }

    public required float MinYaw { get; set; }

    public required float MaxYaw { get; set; }

    public required float YawFriction { get; set; }

    public required float YawBounce { get; set; }

    public required float MinPitch { get; set; }

    public required float MaxPitch { get; set; }

    public required float PitchFriction { get; set; }

    public required float PitchBounce { get; set; }

    public required float BaseMass { get; set; }

    public required float BaseStiffness { get; set; }

    public required float BaseDamping { get; set; }

    public required float BaseMinLeft { get; set; }

    public required float BaseMaxLeft { get; set; }

    public required float BaseLeftFriction { get; set; }

    public required float BaseMinUp { get; set; }

    public required float BaseMaxUp { get; set; }

    public required float BaseUpFriction { get; set; }

    public required float BaseMinForward { get; set; }

    public required float BaseMaxForward { get; set; }

    public required float BaseForwardFriction { get; set; }

    public required float BoingImpactSpeed { get; set; }

    public required float BoingImpactAngle { get; set; }

    public required float BoingDampingRate { get; set; }

    public required float BoingFrequency { get; set; }

    public required float BoingAmplitude { get; set; }
}
