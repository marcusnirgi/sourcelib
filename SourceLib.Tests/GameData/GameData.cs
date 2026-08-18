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
}
