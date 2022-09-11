using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services;
using MegaCorpClash.ViewModels;
using Microsoft.Extensions.Configuration;

ArgumentParser argumentParser = new ArgumentParser();
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
        Console.WriteLine("!exit                Stop the program");
        Console.WriteLine("!bonus <n>           Give bonus <n> points on next tick");
        Console.WriteLine("!clear               Clear console app screen");
        Console.WriteLine("!companies           List player company details");
    }
    else
    {
        var commandWords = command.Split(" ");
        var parsedArguments = argumentParser.Parse(command);

        switch (commandWords[0].ToLowerInvariant())
        {
            case "!bonus":
                if (parsedArguments.IntegerArguments.Count == 1)
                {
                    gameSession
                        .AddBonusPointsNextTurn(parsedArguments.IntegerArguments.First());
                }
                break;
            case "!clear":
                Console.Clear();
                break;
            case "!companies":
                foreach (string player in gameSession.ShowPlayers())
                {
                    Console.WriteLine(player);
                }
                break;
            case "!exit":
                gameSession.End();
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