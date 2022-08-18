using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public interface IHandleChatCommand
{
    string CommandText { get; }
    event EventHandler<MessageEventArgs> OnMessageToDisplay;
    event EventHandler OnPlayerDataUpdated; 
    void Execute(ChatCommand chatCommand);
}