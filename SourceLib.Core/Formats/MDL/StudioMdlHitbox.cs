using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlHitboxSet
{
    public required int NameIndex { get; set; }
    public required int HitboxCount { get; set; }
    public required int HitboxIndex { get; set; }
    public required string Name { get; set; }
    public required IList<StudioMdlHitbox> Hitboxes { get; set; }

    public static StudioMdlHitboxSet ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var hitboxCount = reader.ReadInt32();
        var hitboxIndex = reader.ReadInt32();
        var returnPos = reader.BaseStream.Position;

        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);

        IList<StudioMdlHitbox> hitboxes = [];
        if (hitboxCount > 0 && hitboxIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + hitboxIndex;
            hitboxes = Enumerable
                .Range(0, hitboxCount)
                .Select(_ => StudioMdlHitbox.ReadBinary(reader))
                .ToList();
        }
        reader.BaseStream.Position = returnPos;

        return new StudioMdlHitboxSet
        {
            NameIndex = nameIndex,
            HitboxCount = hitboxCount,
            HitboxIndex = hitboxIndex,
            Name = name,
            Hitboxes = hitboxes,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(HitboxCount);
        writer.Write(HitboxIndex);

        var returnPos = writer.BaseStream.Position;
        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }

        if (HitboxCount > 0 && HitboxIndex != 0 && Hitboxes != null)
        {
            writer.BaseStream.Position = baseOffset + HitboxIndex;
            foreach (var hitbox in Hitboxes)
                hitbox.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlHitbox
{
    public required int Bone { get; set; }
    public required int Group { get; set; }
    public required Vector3 Min { get; set; }
    public required Vector3 Max { get; set; }
    public required int NameIndex { get; set; }
    public required int[] Unused { get; set; }
    public required string Name { get; set; }

    public static StudioMdlHitbox ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var bone = reader.ReadInt32();
        var group = reader.ReadInt32();
        var min = Vector3.ReadBinary(reader);
        var max = Vector3.ReadBinary(reader);
        var nameIndex = reader.ReadInt32();
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
        var name =
            nameIndex != 0
                ? BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0)
                : string.Empty;
        reader.BaseStream.Position = baseOffset + 68;
        return new StudioMdlHitbox
        {
            Bone = bone,
            Group = group,
            Min = min,
            Max = max,
            NameIndex = nameIndex,
            Unused = unused,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(Bone);
        writer.Write(Group);
        writer.Write(Min.X);
        writer.Write(Min.Y);
        writer.Write(Min.Z);
        writer.Write(Max.X);
        writer.Write(Max.Y);
        writer.Write(Max.Z);
        writer.Write(NameIndex);
        foreach (var u in Unused)
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
