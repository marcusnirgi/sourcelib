namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlTexture
{
    public required int NameIndex { get; set; }
    public required string Name { get; set; }
    public required int Flags { get; set; }
    public required int Used { get; set; }
    public required int Unused1 { get; set; }
    public required int Material { get; set; }
    public required int ClientMaterial { get; set; }
    public required int[] Unused { get; set; }

    public static StudioMdlTexture ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var flags = reader.ReadInt32();
        var used = reader.ReadInt32();
        var unused1 = reader.ReadInt32();
        var material = reader.ReadInt32();
        var clientMaterial = reader.ReadInt32();
        var unused = new int[10]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
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
        return new StudioMdlTexture
        {
            NameIndex = nameIndex,
            Name = name,
            Flags = flags,
            Used = used,
            Unused1 = unused1,
            Material = material,
            ClientMaterial = clientMaterial,
            Unused = unused,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(Flags);
        writer.Write(Used);
        writer.Write(Unused1);
        writer.Write(Material);
        writer.Write(ClientMaterial);
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
