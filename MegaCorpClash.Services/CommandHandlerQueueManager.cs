using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Services;

public class CommandHandlerQueueManager : 
    BaseQueueManager<(BaseCommandHandler, GameCommandArgs)>
{
    public event EventHandler<ChatMessageEventArgs> OnChatMessageToSend;
    public event EventHandler OnPlayerDataUpdated;
    public event EventHandler<BankruptedStreamerArgs> OnBankruptedStreamer;

    public CommandHandlerQueueManager()
    {
        Task.Factory.StartNew(Consumer);
    }

    private void Consumer()
    {
        foreach (var item in _queue.GetConsumingEnumerable())
        {
            var commandHandler = item.Item1;
            var commandArgs = item.Item2;
            var chatterDetails = commandHandler.ChatterDetails(commandArgs);

            PublishLogMessage($"[{chatterDetails.ChatterName}] {commandHandler.CommandName} {commandArgs.Argument}");

            commandHandler.Execute(commandArgs);

            foreach (var message in commandHandler.ChatMessages)
            {
                OnChatMessageToSend?.Invoke(this, 
                    new ChatMessageEventArgs(chatterDetails.ChatterName, message));
            }

            if (commandHandler.StreamerBankrupted)
            {
                chatterDetails.Company.VictoryCount++;

                OnBankruptedStreamer?.Invoke(this,
                    new BankruptedStreamerArgs());
            }

            if (commandHandler.PlayerDataUpdated || 
                commandHandler.StreamerBankrupted)
            {
                OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}