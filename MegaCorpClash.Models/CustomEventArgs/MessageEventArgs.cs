using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class MessageEventArgs : BaseCustomEventArgs
{
    public string Message { get; }
    public bool ShowInTwitchChat { get; }

    public MessageEventArgs(ChatCommand chatCommand, string message, 
        bool showInTwitchChat = false)
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
        Message = message;
        ShowInTwitchChat = showInTwitchChat;
    }
}