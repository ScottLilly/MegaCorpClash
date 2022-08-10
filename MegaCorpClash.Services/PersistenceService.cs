using MegaCorpClash.Models;
using Newtonsoft.Json;

namespace MegaCorpClash.Services;

public static class PersistenceService
{
    private const string GAME_SETTINGS_FILE_NAME = @".\appsettings.json";

    public static GameSettings ReadGameSettings()
    {
        if (File.Exists(GAME_SETTINGS_FILE_NAME))
        {
            string text = File.ReadAllText(GAME_SETTINGS_FILE_NAME);

            GameSettings gameSettings = 
                JsonConvert.DeserializeObject<GameSettings>(text);

            if (gameSettings == null)
            {
                throw new FileLoadException($"Error deserializing {GAME_SETTINGS_FILE_NAME}");
            }

            return gameSettings;
        }

        throw new FileNotFoundException(GAME_SETTINGS_FILE_NAME);
    }
}