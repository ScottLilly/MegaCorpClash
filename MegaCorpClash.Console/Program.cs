using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services;
using MegaCorpClash.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using static MegaCorpClash.Models.GameSettings;

var config = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.AddNLog(config);
        }))
    .Build();

NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

ArgumentParser argumentParser = new();
GameSettings gameSettings = GetGameSettings();
GameSession gameSession = new(gameSettings);

Console.WriteLine("Starting MegaCorpClash");

// Wait for user commands
string? command = "";

Logger.Trace("Starting MegaCorpClash");

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
    }
    else
    {
        var commandWords = command.Split(" ");
        var parsedArguments = argumentParser.Parse(command);
        command = commandWords[0].ToLowerInvariant();

        switch (command)
        {
            case "!exit":
                Logger.Trace("Stopping MegaCorpClash");
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
        secretsTwitchAccounts?.FirstOrDefault(ta => ta.Type.Matches("Broadcaster"));
    var secretsBotAccount =
        secretsTwitchAccounts?.FirstOrDefault(ta => ta.Type.Matches("Bot"));

    // Populate missing appsettings value (should only happen in development)
    if (settings.TwitchBroadcasterAccount != null &&
        secretsBroadcasterAccount != null &&
        string.IsNullOrWhiteSpace(settings.TwitchBroadcasterAccount.Name))
    {
        settings.TwitchBroadcasterAccount.Name =
            secretsBroadcasterAccount.Name;
    }

    if (settings.TwitchBroadcasterAccount != null &&
        secretsBroadcasterAccount != null &&
        string.IsNullOrWhiteSpace(settings.TwitchBroadcasterAccount.AuthToken))
    {
        settings.TwitchBroadcasterAccount.AuthToken =
            secretsBroadcasterAccount.AuthToken;
    }

    if (settings.TwitchBotAccount != null &&
        secretsBotAccount != null &&
        string.IsNullOrWhiteSpace(settings.TwitchBotAccount.Name))
    {
        settings.TwitchBotAccount.Name =
            secretsBotAccount.Name;
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