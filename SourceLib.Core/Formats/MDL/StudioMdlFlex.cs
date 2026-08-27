using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlFlexDesc
{
    public required int FacsIndex { get; set; }
    public required string Name { get; set; }

    public static StudioMdlFlexDesc ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var facsIndex = reader.ReadInt32();
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + facsIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlFlexDesc { FacsIndex = facsIndex, Name = name };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(FacsIndex);

        var returnPos = writer.BaseStream.Position;
        if (FacsIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + FacsIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlFlexController
{
    public required int TypeIndex { get; set; }
    public required int NameIndex { get; set; }
    public required int LocalToGlobal { get; set; }
    public required float Min { get; set; }
    public required float Max { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }

    public static StudioMdlFlexController ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var typeIndex = reader.ReadInt32();
        var nameIndex = reader.ReadInt32();
        var localToGlobal = reader.ReadInt32();
        var min = reader.ReadSingle();
        var max = reader.ReadSingle();
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        var type = BinaryReading.ReadStringUntilAt(reader, baseOffset + typeIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlFlexController
        {
            TypeIndex = typeIndex,
            NameIndex = nameIndex,
            LocalToGlobal = localToGlobal,
            Min = min,
            Max = max,
            Name = name,
            Type = type,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(TypeIndex);
        writer.Write(NameIndex);
        writer.Write(LocalToGlobal);
        writer.Write(Min);
        writer.Write(Max);

        var returnPos = writer.BaseStream.Position;
        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }
        if (TypeIndex != 0 && !string.IsNullOrEmpty(Type))
        {
            writer.BaseStream.Position = baseOffset + TypeIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Type));
            writer.Write((byte)0);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlFlexControllerUi
{
    public required int NameIndex { get; set; }
    public required int Index0 { get; set; }
    public required int Index1 { get; set; }
    public required int Index2 { get; set; }
    public required byte RemapType { get; set; }
    public required byte Stereo { get; set; }
    public required byte[] Unused { get; set; }
    public required string Name { get; set; }

    public static StudioMdlFlexControllerUi ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var index0 = reader.ReadInt32();
        var index1 = reader.ReadInt32();
        var index2 = reader.ReadInt32();
        var remapType = reader.ReadByte();
        var stereo = reader.ReadByte();
        var unused = new byte[] { reader.ReadByte(), reader.ReadByte() };
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlFlexControllerUi
        {
            NameIndex = nameIndex,
            Index0 = index0,
            Index1 = index1,
            Index2 = index2,
            RemapType = remapType,
            Stereo = stereo,
            Unused = unused,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(Index0);
        writer.Write(Index1);
        writer.Write(Index2);
        writer.Write(RemapType);
        writer.Write(Stereo);
        writer.Write(Unused[0]);
        writer.Write(Unused[1]);

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

public sealed class StudioMdlFlexOp
{
    public required int Op { get; set; }
    public required int D { get; set; }

    public static StudioMdlFlexOp ReadBinary(BinaryReader reader) =>
        new() { Op = reader.ReadInt32(), D = reader.ReadInt32() };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Op);
        writer.Write(D);
    }
}

public sealed class StudioMdlFlexRule
{
    public required int Flex { get; set; }
    public required int NumOps { get; set; }
    public required int OpIndex { get; set; }
    public required IList<StudioMdlFlexOp> Ops { get; set; }

    public static StudioMdlFlexRule ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var flex = reader.ReadInt32();
        var numOps = reader.ReadInt32();
        var opIndex = reader.ReadInt32();
        IList<StudioMdlFlexOp> ops = [];
        if (numOps > 0 && opIndex != 0)
        {
            var returnPos = reader.BaseStream.Position;
            reader.BaseStream.Position = baseOffset + opIndex;
            ops = Enumerable
                .Range(0, numOps)
                .Select(_ => StudioMdlFlexOp.ReadBinary(reader))
                .ToList();
            reader.BaseStream.Position = returnPos;
        }
        return new StudioMdlFlexRule
        {
            Flex = flex,
            NumOps = numOps,
            OpIndex = opIndex,
            Ops = ops,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(Flex);
        writer.Write(NumOps);
        writer.Write(OpIndex);

        var returnPos = writer.BaseStream.Position;
        if (NumOps > 0 && OpIndex != 0 && Ops != null)
        {
            writer.BaseStream.Position = baseOffset + OpIndex;
            foreach (var op in Ops)
                op.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlFlex
{
    public required int FlexDesc { get; set; }
    public required float Target0 { get; set; }
    public required float Target1 { get; set; }
    public required float Target2 { get; set; }
    public required float Target3 { get; set; }
    public required int NumVerts { get; set; }
    public required int VertIndex { get; set; }
    public required int FlexPair { get; set; }
    public required byte VertAnimType { get; set; }
    public required byte[] UnusedChar { get; set; }
    public required int[] Unused { get; set; }

    public static StudioMdlFlex ReadBinary(BinaryReader reader) =>
        new()
        {
            FlexDesc = reader.ReadInt32(),
            Target0 = reader.ReadSingle(),
            Target1 = reader.ReadSingle(),
            Target2 = reader.ReadSingle(),
            Target3 = reader.ReadSingle(),
            NumVerts = reader.ReadInt32(),
            VertIndex = reader.ReadInt32(),
            FlexPair = reader.ReadInt32(),
            VertAnimType = reader.ReadByte(),
            UnusedChar = [reader.ReadByte(), reader.ReadByte(), reader.ReadByte()],
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

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(FlexDesc);
        writer.Write(Target0);
        writer.Write(Target1);
        writer.Write(Target2);
        writer.Write(Target3);
        writer.Write(NumVerts);
        writer.Write(VertIndex);
        writer.Write(FlexPair);
        writer.Write(VertAnimType);
        writer.Write(UnusedChar[0]);
        writer.Write(UnusedChar[1]);
        writer.Write(UnusedChar[2]);
        foreach (var u in Unused)
            writer.Write(u);
    }
}

public sealed class StudioMdlVertAnim
{
    public required ushort Index { get; set; }
    public required byte Speed { get; set; }
    public required byte Side { get; set; }
    public required short[] Delta { get; set; }
    public required short[] NormalDelta { get; set; }

    public static StudioMdlVertAnim ReadBinary(BinaryReader reader) =>
        new()
        {
            Index = reader.ReadUInt16(),
            Speed = reader.ReadByte(),
            Side = reader.ReadByte(),
            Delta = [reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()],
            NormalDelta = [reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()],
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Speed);
        writer.Write(Side);
        foreach (var d in Delta)
            writer.Write(d);
        foreach (var n in NormalDelta)
            writer.Write(n);
    }
}

public sealed class StudioMdlVertAnimWrinkle
{
    public required ushort Index { get; set; }
    public required byte Speed { get; set; }
    public required byte Side { get; set; }
    public required short[] Delta { get; set; }
    public required short[] NormalDelta { get; set; }
    public required short WrinkleDelta { get; set; }

    public static StudioMdlVertAnimWrinkle ReadBinary(BinaryReader reader) =>
        new()
        {
            Index = reader.ReadUInt16(),
            Speed = reader.ReadByte(),
            Side = reader.ReadByte(),
            Delta = [reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()],
            NormalDelta = [reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()],
            WrinkleDelta = reader.ReadInt16(),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Speed);
        writer.Write(Side);
        foreach (var d in Delta)
            writer.Write(d);
        foreach (var n in NormalDelta)
            writer.Write(n);
        writer.Write(WrinkleDelta);
    }
}

public sealed class StudioMdlEyeball
{
    public required int NameIndex { get; set; }
    public required int Bone { get; set; }
    public required Vector3 Org { get; set; }
    public required float ZOffset { get; set; }
    public required float Radius { get; set; }
    public required Vector3 Up { get; set; }
    public required Vector3 Forward { get; set; }
    public required int Texture { get; set; }
    public required int Unused1 { get; set; }
    public required float IrisScale { get; set; }
    public required int Unused2 { get; set; }
    public required int[] UpperFlexDesc { get; set; }
    public required int[] LowerFlexDesc { get; set; }
    public required float[] UpperTarget { get; set; }
    public required float[] LowerTarget { get; set; }
    public required int UpperLidFlexDesc { get; set; }
    public required int LowerLidFlexDesc { get; set; }
    public required int[] Unused { get; set; }
    public required byte NonFacs { get; set; }
    public required byte[] Unused3 { get; set; }
    public required int[] Unused4 { get; set; }
    public required string Name { get; set; }

    public static StudioMdlEyeball ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var bone = reader.ReadInt32();
        var org = Vector3.ReadBinary(reader);
        var zOffset = reader.ReadSingle();
        var radius = reader.ReadSingle();
        var up = Vector3.ReadBinary(reader);
        var forward = Vector3.ReadBinary(reader);
        var texture = reader.ReadInt32();
        var unused1 = reader.ReadInt32();
        var irisScale = reader.ReadSingle();
        var unused2 = reader.ReadInt32();
        var upperFlexDesc = new int[3]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var lowerFlexDesc = new int[3]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var upperTarget = new float[3]
        {
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
        };
        var lowerTarget = new float[3]
        {
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
        };
        var upperLidFlexDesc = reader.ReadInt32();
        var lowerLidFlexDesc = reader.ReadInt32();
        var unused = new int[4]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var nonFacs = reader.ReadByte();
        var unused3 = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
        var unused4 = new int[7]
        {
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
        reader.BaseStream.Position = returnPos;
        return new StudioMdlEyeball
        {
            NameIndex = nameIndex,
            Bone = bone,
            Org = org,
            ZOffset = zOffset,
            Radius = radius,
            Up = up,
            Forward = forward,
            Texture = texture,
            Unused1 = unused1,
            IrisScale = irisScale,
            Unused2 = unused2,
            UpperFlexDesc = upperFlexDesc,
            LowerFlexDesc = lowerFlexDesc,
            UpperTarget = upperTarget,
            LowerTarget = lowerTarget,
            UpperLidFlexDesc = upperLidFlexDesc,
            LowerLidFlexDesc = lowerLidFlexDesc,
            Unused = unused,
            NonFacs = nonFacs,
            Unused3 = unused3,
            Unused4 = unused4,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(Bone);
        writer.Write(Org.X);
        writer.Write(Org.Y);
        writer.Write(Org.Z);
        writer.Write(ZOffset);
        writer.Write(Radius);
        writer.Write(Up.X);
        writer.Write(Up.Y);
        writer.Write(Up.Z);
        writer.Write(Forward.X);
        writer.Write(Forward.Y);
        writer.Write(Forward.Z);
        writer.Write(Texture);
        writer.Write(Unused1);
        writer.Write(IrisScale);
        writer.Write(Unused2);
        foreach (var u in UpperFlexDesc)
            writer.Write(u);
        foreach (var l in LowerFlexDesc)
            writer.Write(l);
        foreach (var u in UpperTarget)
            writer.Write(u);
        foreach (var l in LowerTarget)
            writer.Write(l);
        writer.Write(UpperLidFlexDesc);
        writer.Write(LowerLidFlexDesc);
        foreach (var u in Unused)
            writer.Write(u);
        writer.Write(NonFacs);
        writer.Write(Unused3[0]);
        writer.Write(Unused3[1]);
        writer.Write(Unused3[2]);
        foreach (var u in Unused4)
            writer.Write(u);

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
