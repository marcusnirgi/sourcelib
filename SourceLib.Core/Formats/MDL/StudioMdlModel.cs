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
}