using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Services;

namespace MegaCorpClash.ViewModels;

public class GameSession : IDisposable
{
    private readonly GameSettings _gameSettings;
    private TwitchConnector? _twitchConnector;
    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;
    private readonly LogWriter _logWriter = new();

    private readonly Dictionary<string, Player> _players = new();

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        PopulatePlayers();

        var statusCommandHandler = new StatusCommandHandler();
        statusCommandHandler.OnCompanyStatusRequested += OnCompanyStatusRequested;

        _twitchConnector = new TwitchConnector(gameSettings, statusCommandHandler);
        _twitchConnector.OnMessageToLog += OnTwitchMessageToLog;
        _twitchConnector.OnCompanyCreated += OnCompanyCreated;
        _twitchConnector.OnCompanyNameChanged += OnCompanyNameChanged;
        _twitchConnector.Connect();

        InitializeTimedMessages(gameSettings.TimedMessages);

        WriteToLog("Game started");
    }

    public void Dispose()
    {
        _twitchConnector?.Dispose();
        _twitchConnector = null;
    }

    private void PopulatePlayers()
    {
        var players = PersistenceService.GetPlayerData();

        foreach (Player player in players)
        {
            _players.Add(player.Id, player);
        }
    }

    private void InitializeTimedMessages(
        GameSettings.TimedMessageSettings timedMessageSettings)
    {
        if (timedMessageSettings.IntervalInMinutes <= 0)
        {
            return;
        }

        _timedMessages = timedMessageSettings.Messages;

        _timedMessagesTimer =
            new System.Timers.Timer(timedMessageSettings.IntervalInMinutes * 60 * 1000);
        _timedMessagesTimer.Elapsed += TimedMessagesTimer_Elapsed;
        _timedMessagesTimer.Enabled = true;
    }

    #region Chat command event handlers

    private void OnCompanyCreated(object? sender, CompanyCreatedEventArgs e)
    {
        _players.TryGetValue(e.TwitchId, out Player? player);

        if (player == null)
        {
            if (_players.Values.Any(p => p.CompanyName.Matches(e.CompanyName)))
            {
                _twitchConnector?
                    .SendChatMessage($"{e.TwitchDisplayName}, there is already a company named {e.CompanyName}");

                return;
            }

            player = new Player
            {
                Id = e.TwitchId,
                DisplayName = e.TwitchDisplayName,
                CompanyName = e.CompanyName,
                CreatedOn = DateTime.UtcNow
            };

            _players[e.TwitchId] = player;

            PersistenceService.SavePlayerData(_players.Values);

            _twitchConnector?
                .SendChatMessage($"{e.TwitchDisplayName}, you are now the proud CEO of {player.CompanyName}");
        }
        else
        {
            _twitchConnector?
                .SendChatMessage($"{e.TwitchDisplayName}, you already have a company name {player.CompanyName}");
        }
    }

    private void OnCompanyNameChanged(object? sender, CompanyNameChangedEventArgs e)
    {
        _players.TryGetValue(e.TwitchId, out Player? player);

        if (player == null)
        {
            // TODO: Start a company for them?
            _twitchConnector?
                .SendChatMessage($"{e.TwitchDisplayName}, you don't have a company. Type !incorporate <company name> to start one.");
        }
        else
        {
            if (_players.Values.Any(p => p.CompanyName.Matches(e.CompanyName)))
            {
                _twitchConnector?
                    .SendChatMessage($"{e.TwitchDisplayName}, there is already a company named {e.CompanyName}");

                return;
            }

            player.CompanyName = e.CompanyName;

            PersistenceService.SavePlayerData(_players.Values);
        }
    }

    private void OnCompanyStatusRequested(object? sender, ChatterEventArgs e)
    {
        _players.TryGetValue(e.TwitchId, out Player? player);

        _twitchConnector?
            .SendChatMessage(player == null
                ? $"{e.TwitchDisplayName}, you do not have a company"
                : $"{e.TwitchDisplayName}: Your company {player.CompanyName} has {player.Points} {_gameSettings.PointsName}");
    }

    #endregion

    private void TimedMessagesTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _twitchConnector?
            .SendChatMessage(_timedMessages.RandomElement());
    }

    private void OnTwitchMessageToLog(object? sender, string e)
    {
        WriteToLog(e);
    }

    private void WriteToLog(string message)
    {
        Console.WriteLine(message);
        _logWriter.WriteMessage(message);
    }
}