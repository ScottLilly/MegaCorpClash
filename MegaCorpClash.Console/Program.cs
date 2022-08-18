using MegaCorpClash.Models;
using MegaCorpClash.Services;
using MegaCorpClash.ViewModels;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Starting MegaCorpClash");

GameSettings gameSettings = GetGameSettings();
GameSession gameSession = new GameSession(gameSettings);

// Wait for user commands
string? command = "";

do
{
    command = Console.ReadLine();

    if (command == null)
    {
        continue;
    }

    if (command.Equals("!help"))
    {
        Console.WriteLine("!exit                Stop te program");
        Console.WriteLine("!clear               Clear console app screen");
    }
    else
    {
        var commandWords = command.Split(" ");

        switch (commandWords[0].ToLowerInvariant())
        {
            case "!clear":
                Console.Clear();
                break;
            case "!exit":
                gameSession.Dispose();
                break;
            default:
                Console.WriteLine($"Unrecognized command: '{command}'");
                break;
        }
    }

} while (!(command ?? "").Equals("!exit", StringComparison.InvariantCultureIgnoreCase));

GameSettings GetGameSettings()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddUserSecrets<Program>(true);

    var configuration = builder.Build();

    string userSecretsToken =
        configuration
            .AsEnumerable()
            .Where(c => c.Key == "TwitchToken")
            .First(c => !string.IsNullOrWhiteSpace(c.Value))
            .Value;

    GameSettings settings =
        PersistenceService.ReadGameSettings();

    if (string.IsNullOrWhiteSpace(settings.TwitchToken) &&
        !string.IsNullOrWhiteSpace(userSecretsToken))
    {
        settings.TwitchToken = userSecretsToken;
    }

    return settings;
}