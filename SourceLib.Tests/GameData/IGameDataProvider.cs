namespace SourceLib.Tests.GameData;

public interface IGameDataProvider
{
    GameData Get(GameId gameId);
}
