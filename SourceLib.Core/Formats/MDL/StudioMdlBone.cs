using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlBone
{
    public required int NameIndex { get; set; }
    public required int Parent { get; set; }
    public required int[] BoneController { get; set; }
    public required Vector3 Position { get; set; }
    public required Quaternion Quaternion { get; set; }
    public required Vector3 Rotation { get; set; }
    public required Vector3 PositionScale { get; set; }
    public required Vector3 RotationScale { get; set; }
    public required Matrix3x4 PoseToBone { get; set; }
    public required Quaternion AlignmentQuaternion { get; set; }
    public required StudioMdlBoneFlags Flags { get; set; }
    public required int ProcedureType { get; set; }
    public required int ProcedureIndex { get; set; }
    public required int PhysicsBone { get; set; }
    public required int SurfacePropIndex { get; set; }
    public required int Contents { get; set; }
    public required int[] Unused { get; set; }

    public required string Name { get; set; }
    public required string SurfaceProp { get; set; }
    public IStudioMdlBoneProcedure? Procedural { get; set; }

    public static StudioMdlBone ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var parent = reader.ReadInt32();
        var boneController = new int[6]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var pos = Vector3.ReadBinary(reader);
        var quat = Quaternion.ReadBinary(reader);
        var rot = Vector3.ReadBinary(reader);
        var posScale = Vector3.ReadBinary(reader);
        var rotScale = Vector3.ReadBinary(reader);
        var poseToBone = Matrix3x4.ReadBinary(reader);
        var alignQuat = Quaternion.ReadBinary(reader);
        var flags = (StudioMdlBoneFlags)reader.ReadInt32();
        var procType = reader.ReadInt32();
        var procIndex = reader.ReadInt32();
        var physicsBone = reader.ReadInt32();
        var surfacePropIndex = reader.ReadInt32();
        var contents = reader.ReadInt32();
        var unused = new int[8]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var returnPos = reader.BaseStream.Position;

        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        var surfaceProp =
            surfacePropIndex != 0
                ? BinaryReading.ReadStringUntilAt(reader, baseOffset + surfacePropIndex, 0)
                : string.Empty;

        IStudioMdlBoneProcedure? procedural = null;
        if (procIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + procIndex;
            procedural = StudioMdlBoneProcedure.ReadBinary(reader, procType);
        }
        reader.BaseStream.Position = returnPos;

        return new StudioMdlBone
        {
            NameIndex = nameIndex,
            Parent = parent,
            BoneController = boneController,
            Position = pos,
            Quaternion = quat,
            Rotation = rot,
            PositionScale = posScale,
            RotationScale = rotScale,
            PoseToBone = poseToBone,
            AlignmentQuaternion = alignQuat,
            Flags = flags,
            ProcedureType = procType,
            ProcedureIndex = procIndex,
            PhysicsBone = physicsBone,
            SurfacePropIndex = surfacePropIndex,
            Contents = contents,
            Unused = unused,
            Name = name,
            SurfaceProp = surfaceProp,
            Procedural = procedural,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(Parent);
        foreach (var bc in BoneController)
            writer.Write(bc);
        writer.Write(Position.X);
        writer.Write(Position.Y);
        writer.Write(Position.Z);
        writer.Write(Quaternion.X);
        writer.Write(Quaternion.Y);
        writer.Write(Quaternion.Z);
        writer.Write(Quaternion.W);
        writer.Write(Rotation.X);
        writer.Write(Rotation.Y);
        writer.Write(Rotation.Z);
        writer.Write(PositionScale.X);
        writer.Write(PositionScale.Y);
        writer.Write(PositionScale.Z);
        writer.Write(RotationScale.X);
        writer.Write(RotationScale.Y);
        writer.Write(RotationScale.Z);
        foreach (var v in PoseToBone.Values)
            writer.Write(v);
        writer.Write(AlignmentQuaternion.X);
        writer.Write(AlignmentQuaternion.Y);
        writer.Write(AlignmentQuaternion.Z);
        writer.Write(AlignmentQuaternion.W);
        writer.Write((int)Flags);
        writer.Write(ProcedureType);
        writer.Write(ProcedureIndex);
        writer.Write(PhysicsBone);
        writer.Write(SurfacePropIndex);
        writer.Write(Contents);
        foreach (var u in Unused)
            writer.Write(u);

        var returnPos = writer.BaseStream.Position;
        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }
        if (SurfacePropIndex != 0 && !string.IsNullOrEmpty(SurfaceProp))
        {
            writer.BaseStream.Position = baseOffset + SurfacePropIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(SurfaceProp));
            writer.Write((byte)0);
        }
        if (ProcedureIndex != 0 && Procedural != null)
        {
            writer.BaseStream.Position = baseOffset + ProcedureIndex;
            Procedural.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlLinearBone
{
    public required int BoneCount { get; set; }
    public required int FlagsIndex { get; set; }
    public required int ParentIndex { get; set; }
    public required int PositionIndex { get; set; }
    public required int QuaternionIndex { get; set; }
    public required int RotationIndex { get; set; }
    public required int PoseToBoneIndex { get; set; }
    public required int PositionScaleIndex { get; set; }
    public required int RotationScaleIndex { get; set; }
    public required int AlignmentQuaternionIndex { get; set; }
    public required int[] Unused { get; set; }

    public static StudioMdlLinearBone ReadBinary(BinaryReader reader)
    {
        return new StudioMdlLinearBone
        {
            BoneCount = reader.ReadInt32(),
            FlagsIndex = reader.ReadInt32(),
            ParentIndex = reader.ReadInt32(),
            PositionIndex = reader.ReadInt32(),
            QuaternionIndex = reader.ReadInt32(),
            RotationIndex = reader.ReadInt32(),
            PoseToBoneIndex = reader.ReadInt32(),
            PositionScaleIndex = reader.ReadInt32(),
            RotationScaleIndex = reader.ReadInt32(),
            AlignmentQuaternionIndex = reader.ReadInt32(),
            Unused = new int[6]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            },
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(BoneCount);
        writer.Write(FlagsIndex);
        writer.Write(ParentIndex);
        writer.Write(PositionIndex);
        writer.Write(QuaternionIndex);
        writer.Write(RotationIndex);
        writer.Write(PoseToBoneIndex);
        writer.Write(PositionScaleIndex);
        writer.Write(RotationScaleIndex);
        writer.Write(AlignmentQuaternionIndex);
        foreach (var u in Unused)
            writer.Write(u);
    }
}

public sealed class StudioMdlSourceBoneTransform
{
    public required int NameIndex { get; set; }
    public required Matrix4x4 PreTransform { get; set; }
    public required Matrix4x4 PostTransform { get; set; }
    public required string Name { get; set; }

    public static StudioMdlSourceBoneTransform ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var pre = Matrix4x4.ReadBinary(reader);
        var post = Matrix4x4.ReadBinary(reader);
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlSourceBoneTransform
        {
            NameIndex = nameIndex,
            PreTransform = pre,
            PostTransform = post,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        foreach (var v in PreTransform.Values)
            writer.Write(v);
        foreach (var v in PostTransform.Values)
            writer.Write(v);

        var returnPos = writer.BaseStream.Position;
        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlBoneFlexDriverControl
{
    public required int BoneComponent { get; set; }
    public required int FlexControllerIndex { get; set; }
    public required float Min { get; set; }
    public required float Max { get; set; }

    public static StudioMdlBoneFlexDriverControl ReadBinary(BinaryReader reader) =>
        new()
        {
            BoneComponent = reader.ReadInt32(),
            FlexControllerIndex = reader.ReadInt32(),
            Min = reader.ReadSingle(),
            Max = reader.ReadSingle(),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(BoneComponent);
        writer.Write(FlexControllerIndex);
        writer.Write(Min);
        writer.Write(Max);
    }
}

public sealed class StudioMdlBoneFlexDriver
{
    public required int BoneIndex { get; set; }
    public required int ControlCount { get; set; }
    public required int ControlIndex { get; set; }
    public required int[] Unused { get; set; }
    public IList<StudioMdlBoneFlexDriverControl>? Controls { get; set; }

    public static StudioMdlBoneFlexDriver ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var boneIndex = reader.ReadInt32();
        var controlCount = reader.ReadInt32();
        var controlIndex = reader.ReadInt32();
        var unused = new int[3] { reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32() };
        IList<StudioMdlBoneFlexDriverControl>? controls = null;
        if (controlCount > 0 && controlIndex != 0)
        {
            var returnPos = reader.BaseStream.Position;
            reader.BaseStream.Position = baseOffset + controlIndex;
            controls = Enumerable
                .Range(0, controlCount)
                .Select(_ => StudioMdlBoneFlexDriverControl.ReadBinary(reader))
                .ToList();
            reader.BaseStream.Position = returnPos;
        }
        return new StudioMdlBoneFlexDriver
        {
            BoneIndex = boneIndex,
            ControlCount = controlCount,
            ControlIndex = controlIndex,
            Unused = unused,
            Controls = controls,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(BoneIndex);
        writer.Write(ControlCount);
        writer.Write(ControlIndex);
        foreach (var u in Unused)
            writer.Write(u);

        var returnPos = writer.BaseStream.Position;
        if (ControlCount > 0 && ControlIndex != 0 && Controls != null)
        {
            writer.BaseStream.Position = baseOffset + ControlIndex;
            foreach (var c in Controls)
                c.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlBoneController
{
    public required int Bone { get; set; }
    public required int Type { get; set; }
    public required float Start { get; set; }
    public required float End { get; set; }
    public required int Rest { get; set; }
    public required int InputField { get; set; }
    public required int[] Unused { get; set; }

    public static StudioMdlBoneController ReadBinary(BinaryReader reader) =>
        new()
        {
            Bone = reader.ReadInt32(),
            Type = reader.ReadInt32(),
            Start = reader.ReadSingle(),
            End = reader.ReadSingle(),
            Rest = reader.ReadInt32(),
            InputField = reader.ReadInt32(),
            Unused = new int[8]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            },
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Bone);
        writer.Write(Type);
        writer.Write(Start);
        writer.Write(End);
        writer.Write(Rest);
        writer.Write(InputField);
        foreach (var u in Unused)
            writer.Write(u);
    }
}
