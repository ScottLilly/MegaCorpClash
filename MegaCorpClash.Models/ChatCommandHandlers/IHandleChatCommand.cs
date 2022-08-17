using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public interface IHandleChatCommand
{
    string CommandText { get; }
    void Execute(ChatCommand chatCommand);
}