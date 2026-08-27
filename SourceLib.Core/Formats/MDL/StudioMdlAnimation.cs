using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlAnimDesc
{
    public required int BasePtr { get; set; }
    public required int NameIndex { get; set; }
    public required float Fps { get; set; }
    public required int Flags { get; set; }
    public required int NumFrames { get; set; }
    public required int NumMovements { get; set; }
    public required int MovementIndex { get; set; }
    public required int[] Unused1 { get; set; }
    public required int AnimBlock { get; set; }
    public required int AnimIndex { get; set; }
    public required int NumIkRules { get; set; }
    public required int IkRuleIndex { get; set; }
    public required int AnimBlockIkRuleIndex { get; set; }
    public required int NumLocalHierarchy { get; set; }
    public required int LocalHierarchyIndex { get; set; }
    public required int SectionIndex { get; set; }
    public required int SectionFrames { get; set; }
    public required short ZeroFrameSpan { get; set; }
    public required short ZeroFrameCount { get; set; }
    public required int ZeroFrameIndex { get; set; }
    public required float ZeroFrameStallTime { get; set; }

    public required int Offset { get; set; }
    public required string Name { get; set; }

    public required IList<StudioMdlMovement> Movements { get; set; }
    public required IList<StudioMdlAnimSection> Sections { get; set; }
    public required IList<StudioMdlIkRule> IkRules { get; set; }
    public required IList<StudioMdlLocalHierarchy> LocalHierarchy { get; set; }
    public required IList<StudioMdlAnimationNode> Nodes { get; set; }

    public static StudioMdlAnimDesc ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;

        var result = new StudioMdlAnimDesc
        {
            BasePtr = reader.ReadInt32(),
            NameIndex = reader.ReadInt32(),
            Fps = reader.ReadSingle(),
            Flags = reader.ReadInt32(),
            NumFrames = reader.ReadInt32(),
            NumMovements = reader.ReadInt32(),
            MovementIndex = reader.ReadInt32(),

            Unused1 =
            [
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            ],

            AnimBlock = reader.ReadInt32(),
            AnimIndex = reader.ReadInt32(),
            NumIkRules = reader.ReadInt32(),
            IkRuleIndex = reader.ReadInt32(),
            AnimBlockIkRuleIndex = reader.ReadInt32(),
            NumLocalHierarchy = reader.ReadInt32(),
            LocalHierarchyIndex = reader.ReadInt32(),
            SectionIndex = reader.ReadInt32(),
            SectionFrames = reader.ReadInt32(),
            ZeroFrameSpan = reader.ReadInt16(),
            ZeroFrameCount = reader.ReadInt16(),
            ZeroFrameIndex = reader.ReadInt32(),
            ZeroFrameStallTime = reader.ReadSingle(),

            Offset = checked((int)baseOffset),
            Name = string.Empty,
            Movements = [],
            Sections = [],
            IkRules = [],
            LocalHierarchy = [],
            Nodes = [],
        };

        var returnPosition = reader.BaseStream.Position;

        if (result.NameIndex != 0)
        {
            result.Name = BinaryReading.ReadStringUntilAt(reader, baseOffset + result.NameIndex, 0);
        }

        if (result.NumMovements > 0 && result.MovementIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + result.MovementIndex;

            result.Movements = Enumerable
                .Range(0, result.NumMovements)
                .Select(_ => StudioMdlMovement.ReadBinary(reader))
                .ToList();
        }

        if (result.SectionFrames != 0 && result.SectionIndex != 0)
        {
            var sectionCount = (result.NumFrames / result.SectionFrames) + 2;

            reader.BaseStream.Position = baseOffset + result.SectionIndex;

            result.Sections = Enumerable
                .Range(0, sectionCount)
                .Select(_ => StudioMdlAnimSection.ReadBinary(reader))
                .ToList();
        }

        if (result.NumIkRules > 0 && result.IkRuleIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + result.IkRuleIndex;

            result.IkRules = Enumerable
                .Range(0, result.NumIkRules)
                .Select(_ => StudioMdlIkRule.ReadBinary(reader))
                .ToList();
        }

        if (result.NumLocalHierarchy > 0 && result.LocalHierarchyIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + result.LocalHierarchyIndex;

            result.LocalHierarchy = Enumerable
                .Range(0, result.NumLocalHierarchy)
                .Select(_ => StudioMdlLocalHierarchy.ReadBinary(reader))
                .ToList();
        }

        if (result.AnimIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + result.AnimIndex;
            result.Nodes = StudioMdlAnimationNode.ReadChain(reader);
        }

        reader.BaseStream.Position = returnPosition;

        return result;
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;

        writer.Write(BasePtr);
        writer.Write(NameIndex);
        writer.Write(Fps);
        writer.Write(Flags);
        writer.Write(NumFrames);
        writer.Write(NumMovements);
        writer.Write(MovementIndex);

        foreach (var value in Unused1)
            writer.Write(value);

        writer.Write(AnimBlock);
        writer.Write(AnimIndex);
        writer.Write(NumIkRules);
        writer.Write(IkRuleIndex);
        writer.Write(AnimBlockIkRuleIndex);
        writer.Write(NumLocalHierarchy);
        writer.Write(LocalHierarchyIndex);
        writer.Write(SectionIndex);
        writer.Write(SectionFrames);
        writer.Write(ZeroFrameSpan);
        writer.Write(ZeroFrameCount);
        writer.Write(ZeroFrameIndex);
        writer.Write(ZeroFrameStallTime);

        var returnPosition = writer.BaseStream.Position;

        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }

        WriteReferenced(writer, baseOffset, MovementIndex, NumMovements, Movements);

        WriteReferenced(writer, baseOffset, SectionIndex, Sections.Count, Sections);

        WriteReferenced(writer, baseOffset, IkRuleIndex, NumIkRules, IkRules);

        WriteReferenced(writer, baseOffset, LocalHierarchyIndex, NumLocalHierarchy, LocalHierarchy);

        if (AnimIndex != 0 && Nodes.Count > 0)
        {
            foreach (var node in Nodes)
            {
                writer.BaseStream.Position = node.Offset != 0 ? node.Offset : writer.BaseStream.Position;
                node.WriteBinary(writer);
            }
        }

        writer.BaseStream.Position = returnPosition;
    }

    private static void WriteReferenced<T>(
        BinaryWriter writer,
        long baseOffset,
        int relativeOffset,
        int count,
        IList<T> values
    )
        where T : notnull
    {
        if (relativeOffset == 0 || count == 0)
            return;

        if (values.Count < count)
        {
            throw new InvalidDataException(
                $"Expected {count} referenced values, got {values.Count}."
            );
        }

        writer.BaseStream.Position = baseOffset + relativeOffset;

        for (var i = 0; i < count; i++)
        {
            switch (values[i])
            {
                case StudioMdlMovement movement:
                    movement.WriteBinary(writer);
                    break;

                case StudioMdlAnimSection section:
                    section.WriteBinary(writer);
                    break;

                case StudioMdlIkRule ikRule:
                    ikRule.WriteBinary(writer);
                    break;

                case StudioMdlLocalHierarchy hierarchy:
                    hierarchy.WriteBinary(writer);
                    break;

                default:
                    throw new InvalidDataException(
                        $"Unsupported referenced MDL type {typeof(T).Name}."
                    );
            }
        }
    }
}

public sealed class StudioMdlAnimSection
{
    public required int AnimBlock { get; set; }
    public required int AnimIndex { get; set; }

    public static StudioMdlAnimSection ReadBinary(BinaryReader reader) =>
        new() { AnimBlock = reader.ReadInt32(), AnimIndex = reader.ReadInt32() };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(AnimBlock);
        writer.Write(AnimIndex);
    }
}

public sealed class StudioMdlMovement
{
    public required int EndFrame { get; set; }
    public required int MotionFlags { get; set; }
    public required float V0 { get; set; }
    public required float V1 { get; set; }
    public required float Angle { get; set; }
    public required Vector3 Vector { get; set; }
    public required Vector3 Position { get; set; }

    public static StudioMdlMovement ReadBinary(BinaryReader reader) =>
        new()
        {
            EndFrame = reader.ReadInt32(),
            MotionFlags = reader.ReadInt32(),
            V0 = reader.ReadSingle(),
            V1 = reader.ReadSingle(),
            Angle = reader.ReadSingle(),
            Vector = Vector3.ReadBinary(reader),
            Position = Vector3.ReadBinary(reader),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(EndFrame);
        writer.Write(MotionFlags);
        writer.Write(V0);
        writer.Write(V1);
        writer.Write(Angle);
        Vector.WriteBinary(writer);
        Position.WriteBinary(writer);
    }
}

public sealed class StudioMdlAnimValuePtr
{
    public required short[] Offset { get; set; }

    public static StudioMdlAnimValuePtr ReadBinary(BinaryReader reader) =>
        new() { Offset = [reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()] };

    public void WriteBinary(BinaryWriter writer)
    {
        foreach (var value in Offset)
            writer.Write(value);
    }
}

public sealed class StudioMdlAnim
{
    public required byte Bone { get; set; }
    public required byte Flags { get; set; }
    public required short NextOffset { get; set; }
    public required List<StudioMdlAnimationNode> Nodes { get; set; }

    public static StudioMdlAnim ReadBinary(BinaryReader reader)
    {
        var node = StudioMdlAnimationNode.ReadBinary(reader);

        return new StudioMdlAnim
        {
            Bone = node.Bone,
            Flags = node.Flags,
            NextOffset = node.NextOffset,
            Nodes = [node],
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        foreach (var node in Nodes)
            node.WriteBinary(writer);
    }
}

public sealed class StudioMdlAnimationNode
{
    public required byte Bone { get; set; }
    public required byte Flags { get; set; }
    public required short NextOffset { get; set; }
    public required int Offset { get; set; }
    public required int Extent { get; set; }
    public byte[] Data { get; set; } = [];

    public bool Terminator => Bone == 255;

    public static StudioMdlAnimationNode ReadBinary(BinaryReader reader)
    {
        var offset = checked((int)reader.BaseStream.Position);

        return new StudioMdlAnimationNode
        {
            Bone = reader.ReadByte(),
            Flags = reader.ReadByte(),
            NextOffset = reader.ReadInt16(),
            Offset = offset,
            Extent = 4,
            Data = [],
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Bone);
        writer.Write(Flags);
        writer.Write(NextOffset);
        if (Data != null && Data.Length > 0)
        {
            writer.Write(Data);
        }
    }

    public static List<StudioMdlAnimationNode> ReadChain(BinaryReader reader, int endOffset = 0)
    {
        var nodes = new List<StudioMdlAnimationNode>();
        var visited = new HashSet<long>();

        while (true)
        {
            var offset = reader.BaseStream.Position;

            if (!visited.Add(offset))
                throw new InvalidDataException(
                    $"Animation node chain contains a cycle at offset {offset}."
                );

            var node = ReadBinary(reader);

            if (node.Terminator || node.NextOffset == 0)
            {
                if (endOffset > offset + 4)
                {
                    node.Extent = checked((int)(endOffset - offset));
                    node.Data = reader.ReadBytes(node.Extent - 4);
                }
                nodes.Add(node);
                break;
            }

            var nextOffset = offset + node.NextOffset;

            if (nextOffset < 0 || nextOffset >= reader.BaseStream.Length)
                throw new InvalidDataException(
                    $"Animation node points outside the stream: {nextOffset}."
                );

            node.Extent = checked((int)(nextOffset - offset));
            if (node.Extent > 4)
            {
                node.Data = reader.ReadBytes(node.Extent - 4);
            }
            nodes.Add(node);

            reader.BaseStream.Position = nextOffset;
        }

        return nodes;
    }
}
