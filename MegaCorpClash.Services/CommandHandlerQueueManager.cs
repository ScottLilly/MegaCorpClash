using System.Collections.Concurrent;
using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Services;

public class CommandHandlerQueueManager : 
    BaseQueueManager<(BaseCommandHandler, GameCommandArgs)>
{
    private readonly int _minimumSecondsBetweenCommands;
    private readonly ConcurrentDictionary<string, CommandTimestamp> _lastCommandRun = new();

    public event EventHandler<ChatMessageEventArgs> OnChatMessageToSend;
    public event EventHandler OnPlayerDataUpdated;
    public event EventHandler<BankruptedStreamerArgs> OnBankruptedStreamer;

    public CommandHandlerQueueManager(int minimumSecondsBetweenCommands)
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

            if (_minimumSecondsBetweenCommands > 0 &&
                _lastCommandRun.ContainsKey(chatterDetails.ChatterId))
            {
                if ((DateTime.UtcNow -
                     _lastCommandRun[chatterDetails.ChatterId].TimeChatted).TotalSeconds <
                     _minimumSecondsBetweenCommands)
                {
                    WarnChatterOfThrottlingCondition(chatterDetails);

                    continue;
                }
            }

            _lastCommandRun[chatterDetails.ChatterId] =
                new CommandTimestamp(DateTime.UtcNow, false);

            PublishLogMessage($"[{chatterDetails.ChatterName}] {commandHandler.CommandName} {commandArgs.Argument}");

            commandHandler.Execute(commandArgs);

            foreach (var message in commandHandler.ChatMessages)
            {
                OnChatMessageToSend?.Invoke(this,
                    new ChatMessageEventArgs(chatterDetails.ChatterName, message));
            }

            NotifyIfStreamerBankrupted(commandHandler, chatterDetails);

            NotifyDataNeedsUpdate(commandHandler);
        }
    }

    private void WarnChatterOfThrottlingCondition(
        (string ChatterId, string ChatterName, Company Company) chatterDetails)
    {
        if (_lastCommandRun[chatterDetails.ChatterId].HasBeenWarned)
        {
            return;
        }

        OnChatMessageToSend?.Invoke(this,
            new ChatMessageEventArgs(chatterDetails.ChatterName,
                $"Please wait {_minimumSecondsBetweenCommands} seconds between commands"));

        _lastCommandRun[chatterDetails.ChatterId].HasBeenWarned = true;
    }

    private void NotifyIfStreamerBankrupted(BaseCommandHandler commandHandler, 
        (string ChatterId, string ChatterName, Company Company) chatterDetails)
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