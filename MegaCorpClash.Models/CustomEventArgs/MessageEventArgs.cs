using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class MessageEventArgs : BaseCustomEventArgs
{
    public string Message { get; }

    public MessageEventArgs(ChatCommand chatCommand, string message)
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
        Message = message;
    }
}