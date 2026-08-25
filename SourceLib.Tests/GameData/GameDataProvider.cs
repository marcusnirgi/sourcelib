using System.Text.Json;
using SourceLib.Core.Formats.VPK;
using SourceLib.Tests.GameData;

internal sealed class GameDataProvider : IGameDataProvider
{
    private GameDataConfig Config { get; set; }

    private GameDataProvider(GameDataConfig config)
    {
        Config = config;
    }

    public static GameDataProvider LoadFromConfig(string configJsonPath)
    {
        if (!File.Exists(configJsonPath))
        {
            throw new FileNotFoundException("Config file not found.");
        }

        var configText = File.ReadAllText(configJsonPath);
        try
        {
            var config = JsonSerializer.Deserialize<GameDataConfig>(configText);
            var provider = new GameDataProvider(config!);
            return provider;
        }
        catch
        {
            throw new Exception("Couldn't load config.");
        }
    }

    public GameData Get(GameId gameId)
    {
        var gameName = gameId.ToString();
        Config.Games.TryGetValue(gameId.ToString(), out var gameDir);
        if (gameDir == null)
        {
            throw new Exception($"Game directory for game {gameName} was not provided.");
        }

        if (!Directory.Exists(gameDir))
        {
            throw new Exception(
                $"Game directory for game {gameName} was provided but doesn't exist."
            );
        }

        var gameData = new GameData(gameId, gameDir);
        return gameData;
    }
}
