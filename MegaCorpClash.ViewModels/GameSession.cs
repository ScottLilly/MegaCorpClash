using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Services;

namespace MegaCorpClash.ViewModels;

public class GameSession
{
    private readonly LogWriter _logWriter = new();
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Player> _players = new();
    private readonly Dictionary<string, DateTime> _lastChatTime = new();
    private readonly TwitchConnector? _twitchConnector;

    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        PopulatePlayers();
        InitializeTimedMessages(gameSettings.TimedMessages);

        var chatCommandHandlers = GetChatCommandHandlers();

        _twitchConnector = new TwitchConnector(gameSettings, chatCommandHandlers);
        _twitchConnector.OnConnected += HandleConnected;
        _twitchConnector.OnDisconnected += HandleDisconnected;
        _twitchConnector.OnPersonChatted += HandlePersonChatted;
        _twitchConnector.OnLogMessagePublished += HandleLogMessagePublished;
        _twitchConnector.Connect();
    }

    public List<string> ShowPlayers()
    {
        return _players
            .OrderBy(p => p.Value.DisplayName)
            .Select(p => $"[{p.Value.DisplayName}] {p.Value.CompanyName} - {p.Value.Points}")
            .ToList();
    }

    public List<string> ShowLastChatTimes()
    {
        return _lastChatTime
            .OrderBy(lct => lct.Value)
            .Select(kvp => $"{kvp.Value} - {_players[kvp.Key].DisplayName}")
            .ToList();
    }

    public void End()
    {
        WriteMessageToTwitchChat("Stopping MegaCorpClash game. Thanks for playing!");

        _twitchConnector?.Disconnect();
    }

    #region Private startup functions

    private void PopulatePlayers()
    {
        var players = PersistenceService.GetPlayerData();

        foreach (Player player in players)
        {
            _players.Add(player.Id, player);
        }
    }

    private List<IHandleChatCommand> GetChatCommandHandlers()
    {
        var chatCommandHandlers =
            new List<IHandleChatCommand>();

        var incorporateCommandHandler = new IncorporateCommandHandler(_players);
        incorporateCommandHandler.OnChatMessagePublished += HandleChatMessagePublished;
        incorporateCommandHandler.OnPlayerDataUpdated += HandlePlayerDataUpdated;
        chatCommandHandlers.Add(incorporateCommandHandler);

        var renameCommandHandler = new RenameCommandHandler(_players);
        renameCommandHandler.OnChatMessagePublished += HandleChatMessagePublished;
        renameCommandHandler.OnPlayerDataUpdated += HandlePlayerDataUpdated;
        chatCommandHandlers.Add(renameCommandHandler);

        var statusCommandHandler = new StatusCommandHandler(_players, _gameSettings.PointsName);
        statusCommandHandler.OnChatMessagePublished += HandleChatMessagePublished;
        chatCommandHandlers.Add(statusCommandHandler);

        var companiesCommandHandler = new CompaniesCommandHandler(_players);
        companiesCommandHandler.OnChatMessagePublished += HandleChatMessagePublished;
        chatCommandHandlers.Add(companiesCommandHandler);

        var setMottoCommandHandler = new SetMottoCommandHandler(_players);
        setMottoCommandHandler.OnChatMessagePublished += HandleChatMessagePublished;
        setMottoCommandHandler.OnPlayerDataUpdated += HandlePlayerDataUpdated;
        chatCommandHandlers.Add(setMottoCommandHandler);

        return chatCommandHandlers;
    }

    #endregion

    #region Private event handler functions

    private void HandleConnected(object? sender, EventArgs e)
    {
        WriteMessageToTwitchChat("Started MegaCorpClash game");
    }

    private void HandleDisconnected(object? sender, EventArgs e)
    {
        WriteMessageToTwitchChat("Stopped MegaCorpClash game");
    }

    private void HandlePersonChatted(object? sender, ChattedEventArgs e)
    {
        if (!_players.ContainsKey(e.UserId))
        {
            return;
        }

        _lastChatTime[e.UserId] = DateTime.Now;
    }

    private void HandleLogMessagePublished(object? sender, string e)
    {
        WriteMessageToLog(e);
    }

    private void HandleChatMessagePublished(object? sender, ChatMessageEventArgs e)
    {
        WriteMessageToTwitchChat($"{e.ChatterDisplayName} {e.Message}");
    }

    private void HandlePlayerDataUpdated(object? sender, EventArgs e)
    {
        PersistenceService.SavePlayerData(_players.Values);
    }

    #endregion

    #region Private timer function methods

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

    private void TimedMessagesTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        WriteMessageToTwitchChat(_timedMessages?.RandomElement() ?? "");
    }

    #endregion

    #region Private output/display functions

    private void WriteMessageToTwitchChat(string message)
    {
        _twitchConnector?.SendChatMessage(message);

        WriteMessageToLog(message);
    }

    private void WriteMessageToLog(string message)
    {
        Console.WriteLine(message);
        _logWriter.WriteMessage(message);
    }

    #endregion
}