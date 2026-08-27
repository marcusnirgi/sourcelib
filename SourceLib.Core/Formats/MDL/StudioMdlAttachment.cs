using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlAttachment
{
    public required int NameIndex { get; set; }
    public required uint Flags { get; set; }
    public required int LocalBone { get; set; }
    public required Matrix4x4 Local { get; set; }
    public required int[] Unused { get; set; }
    public required string Name { get; set; }

    public static StudioMdlAttachment ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var flags = reader.ReadUInt32();
        var localBone = reader.ReadInt32();
        var local = Matrix4x4.ReadBinary(reader);
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
        reader.BaseStream.Position = returnPos;
        return new StudioMdlAttachment
        {
            NameIndex = nameIndex,
            Flags = flags,
            LocalBone = localBone,
            Local = local,
            Unused = unused,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(Flags);
        writer.Write(LocalBone);
        foreach (var v in Local.Values)
            writer.Write(v);
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
