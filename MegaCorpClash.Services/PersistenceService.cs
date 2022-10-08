using MegaCorpClash.Models;
using Newtonsoft.Json;

namespace MegaCorpClash.Services;

public static class PersistenceService
{
    private const string GAME_SETTINGS_FILE_NAME = @".\appsettings.json";
    private const string PLAYER_DATA_FILE_NAME = @".\playerData.json";

    private static object s_syncLock = new();

    public static GameSettings ReadGameSettings()
    {
        if (!File.Exists(GAME_SETTINGS_FILE_NAME))
        {
            throw new FileNotFoundException(GAME_SETTINGS_FILE_NAME);
        }

        string text = File.ReadAllText(GAME_SETTINGS_FILE_NAME);

        GameSettings? gameSettings = 
            JsonConvert.DeserializeObject<GameSettings>(text);

        if (gameSettings == null)
        {
            throw new FileLoadException($"Error deserializing {GAME_SETTINGS_FILE_NAME}");
        }

        return gameSettings;
    }

    public static void SavePlayerData(IEnumerable<Company> players)
    {
        lock (s_syncLock)
        {
            // Make backup
            var playerDataBackupFile = $"{PLAYER_DATA_FILE_NAME}.backup";

            if (File.Exists(playerDataBackupFile))
            {
                File.Delete(playerDataBackupFile);
            }

            // Backup
            File.Move(PLAYER_DATA_FILE_NAME, playerDataBackupFile);

            // Write file
            File.WriteAllText(PLAYER_DATA_FILE_NAME,
                JsonConvert.SerializeObject(players, Formatting.Indented));
        }
    }

    public static List<Company> GetPlayerData()
    {
        if (File.Exists(PLAYER_DATA_FILE_NAME))
        {
            return JsonConvert.DeserializeObject<List<Company>>(
                File.ReadAllText(PLAYER_DATA_FILE_NAME)) ?? new List<Company>();
        }

        return new List<Company>();
    }
}