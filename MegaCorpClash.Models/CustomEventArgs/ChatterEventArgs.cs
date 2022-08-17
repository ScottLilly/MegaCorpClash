using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class ChatterEventArgs : BaseCustomEventArgs
{
    public ChatterEventArgs(ChatCommand chatCommand)
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
    }
}