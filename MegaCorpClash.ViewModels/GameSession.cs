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
        _twitchConnector.OnLogMessagePublished += HandleLogMessagePublished;
        _twitchConnector.Connect();
    }

    public void End()
    {
        DisplayMessage("Stopping MegaCorpClash game. Thanks for playing!", true);

        _twitchConnector?.Disconnect();
    }

    #region Private startup functions

    private List<IHandleChatCommand> GetChatCommandHandlers()
    {
        var chatCommandHandlers =
            new List<IHandleChatCommand>();

        var incorporateCommandHandler = new IncorporateCommandHandler(_players);
        incorporateCommandHandler.OnMessagePublished += HandleMessagePublished;
        incorporateCommandHandler.OnPlayerDataUpdated += HandlePlayerDataUpdated;
        chatCommandHandlers.Add(incorporateCommandHandler);

        var renameCommandHandler = new RenameCommandHandler(_players);
        renameCommandHandler.OnMessagePublished += HandleMessagePublished;
        renameCommandHandler.OnPlayerDataUpdated += HandlePlayerDataUpdated;
        chatCommandHandlers.Add(renameCommandHandler);

        var statusCommandHandler = new StatusCommandHandler(_players, _gameSettings.PointsName);
        statusCommandHandler.OnMessagePublished += HandleMessagePublished;
        chatCommandHandlers.Add(statusCommandHandler);

        var companiesCommandHandler = new CompaniesCommandHandler(_players);
        companiesCommandHandler.OnMessagePublished += HandleMessagePublished;
        chatCommandHandlers.Add(companiesCommandHandler);

        return chatCommandHandlers;
    }

    private void PopulatePlayers()
    {
        var players = PersistenceService.GetPlayerData();

        foreach (Player player in players)
        {
            _players.Add(player.Id, player);
        }
    }

    #endregion

    #region Private event handler functions

    private void HandleConnected(object? sender, EventArgs e)
    {
        DisplayMessage("Started MegaCorpClash game", true);
    }

    private void HandleDisconnected(object? sender, EventArgs e)
    {
        DisplayMessage("Stopped MegaCorpClash game", true);
    }

    private void HandleLogMessagePublished(object? sender, string e)
    {
        WriteToLog(e);
    }

    private void HandleMessagePublished(object? sender, MessageEventArgs e)
    {
        DisplayMessage(e.Message, true);
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
        DisplayMessage(_timedMessages?.RandomElement() ?? "", true);
    }

    #endregion

    #region Private output/display functions

    private void DisplayMessage(string message, bool writeInTwitchChat)
    {
        if (writeInTwitchChat)
        {
            _twitchConnector?.SendChatMessage(message);
        }

        WriteToLog(message);
    }

    private void WriteToLog(string message)
    {
        Console.WriteLine(message);
        _logWriter.WriteMessage(message);
    }

    #endregion
}