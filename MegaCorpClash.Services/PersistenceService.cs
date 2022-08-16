using MegaCorpClash.Models;
using Newtonsoft.Json;

namespace MegaCorpClash.Services;

public static class PersistenceService
{
    private const string GAME_SETTINGS_FILE_NAME = @".\appsettings.json";
    private const string PLAYER_DATA_FILE_NAME = @".\playerData.json";

    public static GameSettings ReadGameSettings()
    {
        if (File.Exists(GAME_SETTINGS_FILE_NAME))
        {
            string text = File.ReadAllText(GAME_SETTINGS_FILE_NAME);

            GameSettings? gameSettings = 
                JsonConvert.DeserializeObject<GameSettings>(text);

            if (gameSettings == null)
            {
                throw new FileLoadException($"Error deserializing {GAME_SETTINGS_FILE_NAME}");
            }

            return gameSettings;
        }

        throw new FileNotFoundException(GAME_SETTINGS_FILE_NAME);
    }

    public static void SavePlayerData(IEnumerable<Player> players)
    {
        File.WriteAllText(PLAYER_DATA_FILE_NAME,
            JsonConvert.SerializeObject(players, Formatting.Indented));
    }

    public static List<Player> GetPlayerData()
    {
        if (File.Exists(PLAYER_DATA_FILE_NAME))
        {
            return JsonConvert.DeserializeObject<List<Player>>(
                File.ReadAllText(PLAYER_DATA_FILE_NAME)) ?? new List<Player>();
        }

        return new List<Player>();
    }
}