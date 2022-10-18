using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services;
using MegaCorpClash.ViewModels;
using Microsoft.Extensions.Configuration;
using static MegaCorpClash.Models.GameSettings;

ArgumentParser argumentParser = new();
GameSettings gameSettings = GetGameSettings();
GameSession gameSession = new(gameSettings);

Console.WriteLine("Starting MegaCorpClash");

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
        Console.WriteLine("!multipletoday <n>   Give bonus multiplier <n> during whole stream");
        Console.WriteLine("!clear               Clear console app screen");
        Console.WriteLine("!companies           List player company details");
    }
    else
    {
        var commandWords = command.Split(" ");
        var parsedArguments = argumentParser.Parse(command);
        command = commandWords[0].ToLowerInvariant();

        switch (command)
        {
            case "!bonus":
                if (parsedArguments.IntegerArguments.Count == 1)
                {
                    var bonusPoints = parsedArguments.IntegerArguments.First();
                    gameSession.AddBonusPointsNextTurn(bonusPoints);
                    Console.WriteLine($"Bonus of {bonusPoints} will be applied on the next tick");
                }
                else
                {
                    Console.WriteLine("!bonus command requires an integer value to apply");
                }
                break;
            case "!multipletoday":
                if (parsedArguments.IntegerArguments.Count == 1)
                {
                    var multiplier = parsedArguments.IntegerArguments.First();
                    gameSession.SetStreamMultiplier(multiplier);
                    Console.WriteLine($"Stream has a multiplier of {multiplier} for today");
                }
                else
                {
                    Console.WriteLine("!multipletoday command requires an integer value to apply");
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

static GameSettings GetGameSettings()
{
    GameSettings settings =
        PersistenceService.ReadGameSettings();

    // Get values from user secrets, when running in development
    var secrets = new ConfigurationBuilder()
        .AddUserSecrets<Program>(true)
        .Build();

    var secretsTwitchAccounts =
        secrets.GetSection("TwitchAccounts")
            .Get<List<TwitchAccount>>();

    var secretsBroadcasterAccount = 
        secretsTwitchAccounts.FirstOrDefault(ta => ta.Type.Matches("Broadcaster"));
    var secretsBotAccount =
        secretsTwitchAccounts.FirstOrDefault(ta => ta.Type.Matches("Bot"));

    // Populate missing appsettings value (should only happen in development)
    if (settings.TwitchBroadcasterAccount != null &&
        secretsBroadcasterAccount != null &&
        string.IsNullOrWhiteSpace(settings.TwitchBroadcasterAccount.AuthToken))
    {
        settings.TwitchBroadcasterAccount.AuthToken =
            secretsBroadcasterAccount.AuthToken;
    }

    if (settings.TwitchBotAccount != null &&
        secretsBotAccount != null &&
        string.IsNullOrWhiteSpace(settings.TwitchBotAccount.AuthToken))
    {
        settings.TwitchBotAccount.AuthToken =
            secretsBotAccount.AuthToken;
    }

    return settings;
}