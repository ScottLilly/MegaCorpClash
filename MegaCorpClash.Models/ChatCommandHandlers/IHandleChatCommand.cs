using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public interface IHandleChatCommand
{
    string CommandName { get; }
    event EventHandler<MessageEventArgs> OnMessagePublished;
    event EventHandler OnPlayerDataUpdated; 
    void Execute(ChatCommand chatCommand);
}