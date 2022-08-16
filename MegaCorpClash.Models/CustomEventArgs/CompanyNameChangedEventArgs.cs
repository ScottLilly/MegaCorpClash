using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class CompanyNameChangedEventArgs : BaseCustomEventArgs
{
    public string CompanyName { get; }

    public CompanyNameChangedEventArgs(ChatCommand chatCommand)
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
        CompanyName = chatCommand.ArgumentsAsString;
    }
}