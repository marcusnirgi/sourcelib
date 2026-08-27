using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlBodyPart
{
    public required int NameIndex { get; set; }
    public required int ModelCount { get; set; }
    public required int Base { get; set; }
    public required int ModelIndex { get; set; }
    public required string Name { get; set; }
    public required IList<StudioMdlModel> Models { get; set; }

    public static StudioMdlBodyPart ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var nameIndex = reader.ReadInt32();
        var modelCount = reader.ReadInt32();
        var baseValue = reader.ReadInt32();
        var modelIndex = reader.ReadInt32();
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + nameIndex, 0);
        reader.BaseStream.Position = baseOffset + modelIndex;
        var models = Enumerable
            .Range(0, modelCount)
            .Select(_ => StudioMdlModel.ReadBinary(reader))
            .ToList();
        reader.BaseStream.Position = returnPos;
        return new StudioMdlBodyPart
        {
            NameIndex = nameIndex,
            ModelCount = modelCount,
            Base = baseValue,
            ModelIndex = modelIndex,
            Name = name,
            Models = models,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(NameIndex);
        writer.Write(ModelCount);
        writer.Write(Base);
        writer.Write(ModelIndex);

        var returnPos = writer.BaseStream.Position;
        if (NameIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + NameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }

        if (ModelCount > 0 && ModelIndex != 0 && Models != null)
        {
            writer.BaseStream.Position = baseOffset + ModelIndex;
            foreach (var model in Models)
                model.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlModel
{
    public required string Name { get; set; }
    public required int Type { get; set; }
    public required float BoundingRadius { get; set; }
    public required int MeshCount { get; set; }
    public required int MeshIndex { get; set; }
    public required int VertexCount { get; set; }
    public required int VertexIndex { get; set; }
    public required int TangentIndex { get; set; }
    public required int AttachmentCount { get; set; }
    public required int AttachmentIndex { get; set; }
    public required int EyeballCount { get; set; }
    public required int EyeballIndex { get; set; }
    public required int[] VertexData { get; set; }
    public required int[] Unused { get; set; }
    public required IList<StudioMdlMesh> Meshes { get; set; }
    public required IList<StudioMdlEyeball> Eyeballs { get; set; }

    public static StudioMdlModel ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var name = new string(reader.ReadChars(64)).TrimEnd('\0');
        var type = reader.ReadInt32();
        var boundingRadius = reader.ReadSingle();
        var meshCount = reader.ReadInt32();
        var meshIndex = reader.ReadInt32();
        var vertexCount = reader.ReadInt32();
        var vertexIndex = reader.ReadInt32();
        var tangentIndex = reader.ReadInt32();
        var attachmentCount = reader.ReadInt32();
        var attachmentIndex = reader.ReadInt32();
        var eyeballCount = reader.ReadInt32();
        var eyeballIndex = reader.ReadInt32();
        var vertexData = new int[4]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var unused = new int[6]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var returnPos = reader.BaseStream.Position;

        reader.BaseStream.Position = baseOffset + meshIndex;
        var meshes = Enumerable
            .Range(0, meshCount)
            .Select(_ => StudioMdlMesh.ReadBinary(reader))
            .ToList();

        reader.BaseStream.Position = baseOffset + eyeballIndex;
        var eyeballs = Enumerable
            .Range(0, eyeballCount)
            .Select(_ => StudioMdlEyeball.ReadBinary(reader))
            .ToList();

        reader.BaseStream.Position = returnPos;

        return new StudioMdlModel
        {
            Name = name,
            Type = type,
            BoundingRadius = boundingRadius,
            MeshCount = meshCount,
            MeshIndex = meshIndex,
            VertexCount = vertexCount,
            VertexIndex = vertexIndex,
            TangentIndex = tangentIndex,
            AttachmentCount = attachmentCount,
            AttachmentIndex = attachmentIndex,
            EyeballCount = eyeballCount,
            EyeballIndex = eyeballIndex,
            VertexData = vertexData,
            Unused = unused,
            Meshes = meshes,
            Eyeballs = eyeballs,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        var nameBuf = (Name + '\0').PadRight(64, '\0')[..64];
        writer.Write(nameBuf.ToCharArray());
        writer.Write(Type);
        writer.Write(BoundingRadius);
        writer.Write(MeshCount);
        writer.Write(MeshIndex);
        writer.Write(VertexCount);
        writer.Write(VertexIndex);
        writer.Write(TangentIndex);
        writer.Write(AttachmentCount);
        writer.Write(AttachmentIndex);
        writer.Write(EyeballCount);
        writer.Write(EyeballIndex);
        foreach (var v in VertexData)
            writer.Write(v);
        foreach (var u in Unused)
            writer.Write(u);

        var returnPos = writer.BaseStream.Position;
        if (MeshCount > 0 && MeshIndex != 0 && Meshes != null)
        {
            writer.BaseStream.Position = baseOffset + MeshIndex;
            foreach (var mesh in Meshes)
                mesh.WriteBinary(writer);
        }

        if (EyeballCount > 0 && EyeballIndex != 0 && Eyeballs != null)
        {
            writer.BaseStream.Position = baseOffset + EyeballIndex;
            foreach (var eyeball in Eyeballs)
                eyeball.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlMesh
{
    public required int Material { get; set; }
    public required int ModelIndex { get; set; }
    public required int NumVertices { get; set; }
    public required int VertexOffset { get; set; }
    public required int NumFlexes { get; set; }
    public required int FlexIndex { get; set; }
    public required int MaterialType { get; set; }
    public required int MaterialParam { get; set; }
    public required int MeshId { get; set; }
    public required Vector3 Center { get; set; }
    public required int[] VertexData { get; set; }
    public required int[] Unused { get; set; }
    public required IList<StudioMdlFlex> Flexes { get; set; }

    public static StudioMdlMesh ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var material = reader.ReadInt32();
        var modelIndex = reader.ReadInt32();
        var numVertices = reader.ReadInt32();
        var vertexOffset = reader.ReadInt32();
        var numFlexes = reader.ReadInt32();
        var flexIndex = reader.ReadInt32();
        var materialType = reader.ReadInt32();
        var materialParam = reader.ReadInt32();
        var meshId = reader.ReadInt32();
        var center = Vector3.ReadBinary(reader);
        var vertexData = new int[9]
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
        };
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

        IList<StudioMdlFlex> flexes = [];
        if (numFlexes > 0 && flexIndex != 0)
        {
            var returnPos = reader.BaseStream.Position;
            reader.BaseStream.Position = baseOffset + flexIndex;
            flexes = Enumerable
                .Range(0, numFlexes)
                .Select(_ => StudioMdlFlex.ReadBinary(reader))
                .ToList();
            reader.BaseStream.Position = returnPos;
        }

        return new StudioMdlMesh
        {
            Material = material,
            ModelIndex = modelIndex,
            NumVertices = numVertices,
            VertexOffset = vertexOffset,
            NumFlexes = numFlexes,
            FlexIndex = flexIndex,
            MaterialType = materialType,
            MaterialParam = materialParam,
            MeshId = meshId,
            Center = center,
            VertexData = vertexData,
            Unused = unused,
            Flexes = flexes,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(Material);
        writer.Write(ModelIndex);
        writer.Write(NumVertices);
        writer.Write(VertexOffset);
        writer.Write(NumFlexes);
        writer.Write(FlexIndex);
        writer.Write(MaterialType);
        writer.Write(MaterialParam);
        writer.Write(MeshId);
        writer.Write(Center.X);
        writer.Write(Center.Y);
        writer.Write(Center.Z);
        foreach (var v in VertexData)
            writer.Write(v);
        foreach (var u in Unused)
            writer.Write(u);

        var returnPos = writer.BaseStream.Position;
        if (NumFlexes > 0 && FlexIndex != 0 && Flexes != null)
        {
            writer.BaseStream.Position = baseOffset + FlexIndex;
            foreach (var flex in Flexes)
                flex.WriteBinary(writer);
        }
        writer.BaseStream.Position = returnPos;
    }
}
