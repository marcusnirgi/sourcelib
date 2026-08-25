using SourceLib.Core.Formats.VPK;

namespace SourceLib.Tests.GameData;

public sealed class GameData
{
    public GameId GameId { get; }
    public string Root { get; }

    public GameData(GameId gameId, string root)
    {
        GameId = gameId;
        Root = root;
    }

    public string GetPath(params string[] parts)
    {
        return Path.Combine(Root, Path.Join(parts));
    }

    public VPK GetVPK(VPKFormatParser vpkParser, string[] parts, IReadOnlyList<string> chunkPaths)
    {
        var vpkPath = GetPath(parts);
        var vpkBytes = File.ReadAllBytes(vpkPath);
        var chunkStreams = chunkPaths
            .Select(chunkPath => new VPKChunkStream(
                File.ReadAllBytes(Path.Combine(Root, chunkPath))
            ))
            .ToList();

        var vpk = vpkParser.Parse(vpkBytes, chunkStreams);
        return vpk;
    }
}
