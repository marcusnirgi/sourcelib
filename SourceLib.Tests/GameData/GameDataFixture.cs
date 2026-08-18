namespace SourceLib.Tests.GameData;

public sealed class GameDataFixture
{
    public IGameDataProvider Provider { get; } =
        GameDataProvider.LoadFromConfig("sourcelib.local.json");
}
