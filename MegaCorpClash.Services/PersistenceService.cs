using System.Text.Json;
using System.Text.Json.Serialization;
using MegaCorpClash.Models;

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
            JsonSerializer.Deserialize<GameSettings>(text, 
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });

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
                JsonSerializer.Serialize(players, 
                new JsonSerializerOptions 
                {
                    Converters = { new JsonStringEnumConverter() },
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                    WriteIndented = true 
                }));
        }
    }

    public static List<Company> GetPlayerData()
    {
        if (File.Exists(PLAYER_DATA_FILE_NAME))
        {
            return JsonSerializer.Deserialize<List<Company>>(
                File.ReadAllText(PLAYER_DATA_FILE_NAME), 
                new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() },
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }) ?? new List<Company>();
        }

        return new List<Company>();
    }
}