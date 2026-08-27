using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlIkLink
{
    public required int Bone { get; set; }
    public required Vector3 KneeDir { get; set; }
    public required Vector3 Unused0 { get; set; }

    public static StudioMdlIkLink ReadBinary(BinaryReader reader) =>
        new()
        {
            Bone = reader.ReadInt32(),
            KneeDir = Vector3.ReadBinary(reader),
            Unused0 = Vector3.ReadBinary(reader),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Bone);
        writer.Write(KneeDir.X);
        writer.Write(KneeDir.Y);
        writer.Write(KneeDir.Z);
        writer.Write(Unused0.X);
        writer.Write(Unused0.Y);
        writer.Write(Unused0.Z);
    }
}

public sealed class StudioMdlIkChain
{
    public required int NameIndex { get; set; }
    public required int LinkType { get; set; }
    public required int NumLinks { get; set; }
    public required int LinkIndex { get; set; }
    public required IList<StudioMdlIkLink> Links { get; set; }
    public required string Name { get; set; }

    public static StudioMdlIkChain ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var linkType = reader.ReadInt32();
        var numLinks = reader.ReadInt32();
        var linkIndex = reader.ReadInt32();
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        IList<StudioMdlIkLink> links = [];
        if (numLinks > 0 && linkIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + linkIndex;
            links = Enumerable
                .Range(0, numLinks)
                .Select(_ => StudioMdlIkLink.ReadBinary(reader))
                .ToList();
        }
        reader.BaseStream.Position = returnPos;
        return new StudioMdlIkChain
        {
            NameIndex = nameIndex,
            LinkType = linkType,
            NumLinks = numLinks,
            LinkIndex = linkIndex,
            Links = links,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(LinkType);
        writer.Write(NumLinks);
        writer.Write(LinkIndex);

        var returnPos = writer.BaseStream.Position;
        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }
        if (NumLinks > 0 && LinkIndex != 0 && Links != null)
        {
            writer.BaseStream.Position = baseOffset + LinkIndex;
            foreach (var link in Links)
                link.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlIkError
{
    public required Vector3 Position { get; set; }
    public required Quaternion Quaternion { get; set; }

    public static StudioMdlIkError ReadBinary(BinaryReader reader) =>
        new() { Position = Vector3.ReadBinary(reader), Quaternion = Quaternion.ReadBinary(reader) };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Position.X);
        writer.Write(Position.Y);
        writer.Write(Position.Z);
        writer.Write(Quaternion.X);
        writer.Write(Quaternion.Y);
        writer.Write(Quaternion.Z);
        writer.Write(Quaternion.W);
    }
}

public sealed class StudioMdlCompressedIkError
{
    public required float[] Scale { get; set; }
    public required short[] Offset { get; set; }
}

public sealed class StudioMdlIkRule
{
    public required int Index { get; set; }
    public required int Type { get; set; }
    public required int Chain { get; set; }
    public required int Bone { get; set; }
    public required int Slot { get; set; }
    public required float Height { get; set; }
    public required float Radius { get; set; }
    public required float Floor { get; set; }
    public required Vector3 Position { get; set; }
    public required Quaternion Quaternion { get; set; }
    public required int CompressedIkErrorIndex { get; set; }
    public required int Unused2 { get; set; }
    public required int IStart { get; set; }
    public required int IkErrorIndex { get; set; }
    public required float Start { get; set; }
    public required float Peak { get; set; }
    public required float Tail { get; set; }
    public required float End { get; set; }
    public required float Unused3 { get; set; }
    public required float Contact { get; set; }
    public required float Drop { get; set; }
    public required float Top { get; set; }
    public required int Unused6 { get; set; }
    public required int Unused7 { get; set; }
    public required int Unused8 { get; set; }
    public required int AttachmentIndex { get; set; }
    public required int[] Unused { get; set; }
    public required string Attachment { get; set; }

    public static StudioMdlIkRule ReadBinary(BinaryReader reader)
    {
        var ruleOffset = reader.BaseStream.Position;
        var index = reader.ReadInt32();
        var type = reader.ReadInt32();
        var chain = reader.ReadInt32();
        var bone = reader.ReadInt32();
        var slot = reader.ReadInt32();
        var height = reader.ReadSingle();
        var radius = reader.ReadSingle();
        var floor = reader.ReadSingle();
        var pos = Vector3.ReadBinary(reader);
        var quat = Quaternion.ReadBinary(reader);
        var compressedIkErrorIndex = reader.ReadInt32();
        var unused2 = reader.ReadInt32();
        var iStart = reader.ReadInt32();
        var ikErrorIndex = reader.ReadInt32();
        var start = reader.ReadSingle();
        var peak = reader.ReadSingle();
        var tail = reader.ReadSingle();
        var end = reader.ReadSingle();
        var unused3 = reader.ReadSingle();
        var contact = reader.ReadSingle();
        var drop = reader.ReadSingle();
        var top = reader.ReadSingle();
        var unused6 = reader.ReadInt32();
        var unused7 = reader.ReadInt32();
        var unused8 = reader.ReadInt32();
        var attachmentIndex = reader.ReadInt32();
        var unusedArr = new int[7]
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
        var attachment = BinaryReading.ReadStringUntilAt(reader, ruleOffset + attachmentIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlIkRule
        {
            Index = index,
            Type = type,
            Chain = chain,
            Bone = bone,
            Slot = slot,
            Height = height,
            Radius = radius,
            Floor = floor,
            Position = pos,
            Quaternion = quat,
            CompressedIkErrorIndex = compressedIkErrorIndex,
            Unused2 = unused2,
            IStart = iStart,
            IkErrorIndex = ikErrorIndex,
            Start = start,
            Peak = peak,
            Tail = tail,
            End = end,
            Unused3 = unused3,
            Contact = contact,
            Drop = drop,
            Top = top,
            Unused6 = unused6,
            Unused7 = unused7,
            Unused8 = unused8,
            AttachmentIndex = attachmentIndex,
            Unused = unusedArr,
            Attachment = attachment,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(Index);
        writer.Write(Type);
        writer.Write(Chain);
        writer.Write(Bone);
        writer.Write(Slot);
        writer.Write(Height);
        writer.Write(Radius);
        writer.Write(Floor);
        writer.Write(Position.X);
        writer.Write(Position.Y);
        writer.Write(Position.Z);
        writer.Write(Quaternion.X);
        writer.Write(Quaternion.Y);
        writer.Write(Quaternion.Z);
        writer.Write(Quaternion.W);
        writer.Write(CompressedIkErrorIndex);
        writer.Write(Unused2);
        writer.Write(IStart);
        writer.Write(IkErrorIndex);
        writer.Write(Start);
        writer.Write(Peak);
        writer.Write(Tail);
        writer.Write(End);
        writer.Write(Unused3);
        writer.Write(Contact);
        writer.Write(Drop);
        writer.Write(Top);
        writer.Write(Unused6);
        writer.Write(Unused7);
        writer.Write(Unused8);
        writer.Write(AttachmentIndex);
        foreach (var u in Unused)
            writer.Write(u);

        var returnPos = writer.BaseStream.Position;
        if (AttachmentIndex != 0 && !string.IsNullOrEmpty(Attachment))
        {
            writer.BaseStream.Position = baseOffset + AttachmentIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Attachment));
            writer.Write((byte)0);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlLocalHierarchy
{
    public required int Bone { get; set; }
    public required int NewParent { get; set; }
    public required float Start { get; set; }
    public required float Peak { get; set; }
    public required float Tail { get; set; }
    public required float End { get; set; }
    public required int IStart { get; set; }
    public required int LocalAnimIndex { get; set; }
    public required int[] Unused { get; set; }

    public static StudioMdlLocalHierarchy ReadBinary(BinaryReader reader) =>
        new()
        {
            Bone = reader.ReadInt32(),
            NewParent = reader.ReadInt32(),
            Start = reader.ReadSingle(),
            Peak = reader.ReadSingle(),
            Tail = reader.ReadSingle(),
            End = reader.ReadSingle(),
            IStart = reader.ReadInt32(),
            LocalAnimIndex = reader.ReadInt32(),
            Unused = new int[4]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            },
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Bone);
        writer.Write(NewParent);
        writer.Write(Start);
        writer.Write(Peak);
        writer.Write(Tail);
        writer.Write(End);
        writer.Write(IStart);
        writer.Write(LocalAnimIndex);
        foreach (var u in Unused)
            writer.Write(u);
    }
}
