using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public interface IStudioMdlBoneProcedure
{
    void WriteBinary(BinaryWriter writer);
}

public static class StudioMdlBoneProcedure
{
    public static IStudioMdlBoneProcedure? ReadBinary(BinaryReader reader, int procedureType)
    {
        return procedureType switch
        {
            1 => StudioMdlAxisInterpBone.ReadBinary(reader),
            2 => StudioMdlQuatInterpBone.ReadBinary(reader),
            3 or 4 => StudioMdlAimAtBone.ReadBinary(reader),
            5 => StudioMdlJiggleBone.ReadBinary(reader),
            _ => throw new InvalidDataException($"Unknown bone procedure type: {procedureType}"),
        };
    }
}

public sealed class StudioMdlAxisInterpBone : IStudioMdlBoneProcedure
{
    public required int Control { get; set; }
    public required int Axis { get; set; }
    public required Vector3[] Position { get; set; }
    public required Quaternion[] Quat { get; set; }

    public static StudioMdlAxisInterpBone ReadBinary(BinaryReader reader) =>
        new()
        {
            Control = reader.ReadInt32(),
            Axis = reader.ReadInt32(),
            Position =
            [
                Vector3.ReadBinary(reader),
                Vector3.ReadBinary(reader),
                Vector3.ReadBinary(reader),
                Vector3.ReadBinary(reader),
                Vector3.ReadBinary(reader),
                Vector3.ReadBinary(reader),
            ],
            Quat =
            [
                Quaternion.ReadBinary(reader),
                Quaternion.ReadBinary(reader),
                Quaternion.ReadBinary(reader),
                Quaternion.ReadBinary(reader),
                Quaternion.ReadBinary(reader),
                Quaternion.ReadBinary(reader),
            ],
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Control);
        writer.Write(Axis);
        foreach (var p in Position)
        {
            writer.Write(p.X);
            writer.Write(p.Y);
            writer.Write(p.Z);
        }
        foreach (var q in Quat)
        {
            writer.Write(q.X);
            writer.Write(q.Y);
            writer.Write(q.Z);
            writer.Write(q.W);
        }
    }
}

public sealed class StudioMdlQuatInterpInfo
{
    public required float InverseTolerance { get; set; }
    public required Quaternion Trigger { get; set; }
    public required Vector3 Position { get; set; }
    public required Quaternion Quaternion { get; set; }

    public static StudioMdlQuatInterpInfo ReadBinary(BinaryReader reader) =>
        new()
        {
            InverseTolerance = reader.ReadSingle(),
            Trigger = Quaternion.ReadBinary(reader),
            Position = Vector3.ReadBinary(reader),
            Quaternion = Quaternion.ReadBinary(reader),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(InverseTolerance);
        writer.Write(Trigger.X);
        writer.Write(Trigger.Y);
        writer.Write(Trigger.Z);
        writer.Write(Trigger.W);
        writer.Write(Position.X);
        writer.Write(Position.Y);
        writer.Write(Position.Z);
        writer.Write(Quaternion.X);
        writer.Write(Quaternion.Y);
        writer.Write(Quaternion.Z);
        writer.Write(Quaternion.W);
    }
}

public sealed class StudioMdlQuatInterpBone : IStudioMdlBoneProcedure
{
    public required int Control { get; set; }
    public required int TriggerCount { get; set; }
    public required int TriggerIndex { get; set; }
    public required StudioMdlQuatInterpInfo[] Triggers { get; set; }

    public static StudioMdlQuatInterpBone ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var control = reader.ReadInt32();
        var triggerCount = reader.ReadInt32();
        var triggerIndex = reader.ReadInt32();
        StudioMdlQuatInterpInfo[] triggers = [];
        if (triggerCount > 0 && triggerIndex != 0)
        {
            var returnPos = reader.BaseStream.Position;
            reader.BaseStream.Position = baseOffset + triggerIndex;
            triggers = Enumerable
                .Range(0, triggerCount)
                .Select(_ => StudioMdlQuatInterpInfo.ReadBinary(reader))
                .ToArray();
            reader.BaseStream.Position = returnPos;
        }
        return new StudioMdlQuatInterpBone
        {
            Control = control,
            TriggerCount = triggerCount,
            TriggerIndex = triggerIndex,
            Triggers = triggers,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(Control);
        writer.Write(TriggerCount);
        writer.Write(TriggerIndex);

        var returnPos = writer.BaseStream.Position;
        if (TriggerCount > 0 && TriggerIndex != 0 && Triggers != null)
        {
            writer.BaseStream.Position = baseOffset + TriggerIndex;
            foreach (var trigger in Triggers)
                trigger.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlAimAtBone : IStudioMdlBoneProcedure
{
    public required int Parent { get; set; }
    public required int Aim { get; set; }
    public required Vector3 AimVector { get; set; }
    public required Vector3 UpVector { get; set; }
    public required Vector3 BasePosition { get; set; }

    public static StudioMdlAimAtBone ReadBinary(BinaryReader reader) =>
        new()
        {
            Parent = reader.ReadInt32(),
            Aim = reader.ReadInt32(),
            AimVector = Vector3.ReadBinary(reader),
            UpVector = Vector3.ReadBinary(reader),
            BasePosition = Vector3.ReadBinary(reader),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Parent);
        writer.Write(Aim);
        writer.Write(AimVector.X);
        writer.Write(AimVector.Y);
        writer.Write(AimVector.Z);
        writer.Write(UpVector.X);
        writer.Write(UpVector.Y);
        writer.Write(UpVector.Z);
        writer.Write(BasePosition.X);
        writer.Write(BasePosition.Y);
        writer.Write(BasePosition.Z);
    }
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

    public static StudioMdlJiggleBone ReadBinary(BinaryReader reader) =>
        new()
        {
            Flags = reader.ReadInt32(),
            Length = reader.ReadSingle(),
            TipMass = reader.ReadSingle(),
            YawStiffness = reader.ReadSingle(),
            YawDamping = reader.ReadSingle(),
            PitchStiffness = reader.ReadSingle(),
            PitchDamping = reader.ReadSingle(),
            AlongStiffness = reader.ReadSingle(),
            AlongDamping = reader.ReadSingle(),
            AngleLimit = reader.ReadSingle(),
            MinYaw = reader.ReadSingle(),
            MaxYaw = reader.ReadSingle(),
            YawFriction = reader.ReadSingle(),
            YawBounce = reader.ReadSingle(),
            MinPitch = reader.ReadSingle(),
            MaxPitch = reader.ReadSingle(),
            PitchFriction = reader.ReadSingle(),
            PitchBounce = reader.ReadSingle(),
            BaseMass = reader.ReadSingle(),
            BaseStiffness = reader.ReadSingle(),
            BaseDamping = reader.ReadSingle(),
            BaseMinLeft = reader.ReadSingle(),
            BaseMaxLeft = reader.ReadSingle(),
            BaseLeftFriction = reader.ReadSingle(),
            BaseMinUp = reader.ReadSingle(),
            BaseMaxUp = reader.ReadSingle(),
            BaseUpFriction = reader.ReadSingle(),
            BaseMinForward = reader.ReadSingle(),
            BaseMaxForward = reader.ReadSingle(),
            BaseForwardFriction = reader.ReadSingle(),
            BoingImpactSpeed = reader.ReadSingle(),
            BoingImpactAngle = reader.ReadSingle(),
            BoingDampingRate = reader.ReadSingle(),
            BoingFrequency = reader.ReadSingle(),
            BoingAmplitude = reader.ReadSingle(),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Flags);
        writer.Write(Length);
        writer.Write(TipMass);
        writer.Write(YawStiffness);
        writer.Write(YawDamping);
        writer.Write(PitchStiffness);
        writer.Write(PitchDamping);
        writer.Write(AlongStiffness);
        writer.Write(AlongDamping);
        writer.Write(AngleLimit);
        writer.Write(MinYaw);
        writer.Write(MaxYaw);
        writer.Write(YawFriction);
        writer.Write(YawBounce);
        writer.Write(MinPitch);
        writer.Write(MaxPitch);
        writer.Write(PitchFriction);
        writer.Write(PitchBounce);
        writer.Write(BaseMass);
        writer.Write(BaseStiffness);
        writer.Write(BaseDamping);
        writer.Write(BaseMinLeft);
        writer.Write(BaseMaxLeft);
        writer.Write(BaseLeftFriction);
        writer.Write(BaseMinUp);
        writer.Write(BaseMaxUp);
        writer.Write(BaseUpFriction);
        writer.Write(BaseMinForward);
        writer.Write(BaseMaxForward);
        writer.Write(BaseForwardFriction);
        writer.Write(BoingImpactSpeed);
        writer.Write(BoingImpactAngle);
        writer.Write(BoingDampingRate);
        writer.Write(BoingFrequency);
        writer.Write(BoingAmplitude);
    }
}
