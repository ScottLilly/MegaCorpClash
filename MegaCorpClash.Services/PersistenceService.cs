using System.Text.Json;
using System.Text.Json.Serialization;
using MegaCorpClash.Models;
using MegaCorpClash.Services.Persistence;

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

    public static List<Company> GetPlayerData()
    {
        IRepository db = CompanyRepository.GetInstance();

        // Uncomment, to reload database from JSON file
        //if (File.Exists(PLAYER_DATA_FILE_NAME))
        //{
        //    var companies = JsonSerializer.Deserialize<List<Company>>(
        //        File.ReadAllText(PLAYER_DATA_FILE_NAME),
        //        new JsonSerializerOptions
        //        {
        //            Converters = { new JsonStringEnumConverter() },
        //            IgnoreReadOnlyFields = true,
        //            IgnoreReadOnlyProperties = true,
        //            NumberHandling = JsonNumberHandling.AllowReadingFromString
        //        }) ?? new List<Company>();

        //    foreach(Company company in companies)
        //    {
        //        db.AddCompany(company);
        //    }
        //}

        return db.GetAllCompanies().ToList();
    }
}