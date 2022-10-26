using System.Collections.Concurrent;
using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.Queues;

public class CommandHandlerQueue :
    BaseTypedQueue<(BaseCommandHandler, GameCommandArgs)>
{
    private readonly int _minimumSecondsBetweenCommands;
    private readonly ConcurrentDictionary<string, CommandTimestamp> _lastCommandTimestamp = new();
    public event EventHandler OnPlayerDataUpdated;
    public event EventHandler<BankruptedStreamerArgs> OnBankruptedStreamer;

    public CommandHandlerQueue(int minimumSecondsBetweenCommands)
    {
        _minimumSecondsBetweenCommands = minimumSecondsBetweenCommands;

        Task.Factory.StartNew(Consumer);
    }

    private void Consumer()
    {
        foreach (var item in _queue.GetConsumingEnumerable())
        {
            var commandHandler = item.Item1;
            var commandArgs = item.Item2;
            var chatterDetails = commandHandler.ChatterDetails(commandArgs);

            _lastCommandTimestamp.TryGetValue(
                chatterDetails.ChatterId, 
                out var chattersLastCommand);

            if (ChatIsThrottled() &&
                chattersLastCommand != null)
            {
                if ((DateTime.UtcNow -
                     chattersLastCommand.TimeChatted).TotalSeconds <
                     _minimumSecondsBetweenCommands)
                {
                    WarnChatterOfThrottlingCondition(chatterDetails);

                    continue;
                }
            }

            _lastCommandTimestamp[chatterDetails.ChatterId] =
                new CommandTimestamp(DateTime.UtcNow, false);

            PublishLogMessage($"[{chatterDetails.ChatterName}] {commandHandler.CommandName} {commandArgs.Argument}");

            commandHandler.Execute(commandArgs);

            foreach (var message in commandHandler.ChatMessages)
            {
                PublishChatMessage(chatterDetails.ChatterName, message);
            }

            NotifyIfStreamerBankrupted(commandHandler, chatterDetails);

            NotifyDataNeedsUpdate(commandHandler);
        }
    }

    private void WarnChatterOfThrottlingCondition(
        ChatterDetails chatterDetails)
    {
        if (_lastCommandTimestamp[chatterDetails.ChatterId].HasBeenWarned)
        {
            return;
        }

        PublishChatMessage(
            chatterDetails.ChatterName,
            $"Please wait {_minimumSecondsBetweenCommands} seconds between commands");

        _lastCommandTimestamp[chatterDetails.ChatterId].HasBeenWarned = true;
    }

    private void NotifyIfStreamerBankrupted(BaseCommandHandler commandHandler,
        ChatterDetails chatterDetails)
    {
        if (commandHandler.StreamerBankrupted)
        {
            chatterDetails.Company.VictoryCount++;

            OnBankruptedStreamer?.Invoke(this,
                new BankruptedStreamerArgs());
        }
    }

    private void NotifyDataNeedsUpdate(BaseCommandHandler commandHandler)
    {
        if (commandHandler.PlayerDataUpdated ||
            commandHandler.StreamerBankrupted)
        {
            OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool ChatIsThrottled() =>
        _minimumSecondsBetweenCommands > 0;

    private class CommandTimestamp
    {
        public DateTime TimeChatted { get; set; }
        public bool HasBeenWarned { get; set; }

        public CommandTimestamp(DateTime timeChatted, bool hasBeenWarned)
        {
            TimeChatted = timeChatted;
            HasBeenWarned = hasBeenWarned;
        }
    }

}