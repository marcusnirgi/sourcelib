namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlPoseParamDesc
{
    public required int NameIndex { get; set; }
    public required int Flags { get; set; }
    public required float Start { get; set; }
    public required float End { get; set; }
    public required float Loop { get; set; }
    public required string Name { get; set; }

    public static StudioMdlPoseParamDesc ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var flags = reader.ReadInt32();
        var start = reader.ReadSingle();
        var end = reader.ReadSingle();
        var loop = reader.ReadSingle();
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlPoseParamDesc
        {
            NameIndex = nameIndex,
            Flags = flags,
            Start = start,
            End = end,
            Loop = loop,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(Flags);
        writer.Write(Start);
        writer.Write(End);
        writer.Write(Loop);

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

public sealed class StudioMdlMouth
{
    public required int Bone { get; set; }
    public required float ForwardX { get; set; }
    public required float ForwardY { get; set; }
    public required float ForwardZ { get; set; }
    public required int FlexDesc { get; set; }

    public static StudioMdlMouth ReadBinary(BinaryReader reader) =>
        new()
        {
            Bone = reader.ReadInt32(),
            ForwardX = reader.ReadSingle(),
            ForwardY = reader.ReadSingle(),
            ForwardZ = reader.ReadSingle(),
            FlexDesc = reader.ReadInt32(),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Bone);
        writer.Write(ForwardX);
        writer.Write(ForwardY);
        writer.Write(ForwardZ);
        writer.Write(FlexDesc);
    }
}

public sealed class StudioMdlModelGroup
{
    public required int LabelIndex { get; set; }
    public required int NameIndex { get; set; }
    public required string Label { get; set; }
    public required string Name { get; set; }

    public static StudioMdlModelGroup ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var labelIndex = reader.ReadInt32();
        var nameIndex = reader.ReadInt32();
        var returnPos = reader.BaseStream.Position;
        var label = BinaryReading.ReadStringUntilAt(reader, baseOffset + labelIndex, 0);
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlModelGroup
        {
            LabelIndex = labelIndex,
            NameIndex = nameIndex,
            Label = label,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(LabelIndex);
        writer.Write(NameIndex);

        var returnPos = writer.BaseStream.Position;
        if (LabelIndex != 0 && !string.IsNullOrEmpty(Label))
        {
            writer.BaseStream.Position = baseOffset + LabelIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Label));
            writer.Write((byte)0);
        }
        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlAnimBlock
{
    public required int DataStart { get; set; }
    public required int DataEnd { get; set; }

    public static StudioMdlAnimBlock ReadBinary(BinaryReader reader) =>
        new() { DataStart = reader.ReadInt32(), DataEnd = reader.ReadInt32() };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(DataStart);
        writer.Write(DataEnd);
    }
}

public sealed class StudioMdlActivityModifier
{
    public required int NameIndex { get; set; }
    public required string Name { get; set; }

    public static StudioMdlActivityModifier ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlActivityModifier { NameIndex = nameIndex, Name = name };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);

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
