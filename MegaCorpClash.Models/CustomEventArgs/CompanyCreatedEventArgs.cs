using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class CompanyCreatedEventArgs : BaseCustomEventArgs
{
    public string CompanyName { get; }

    public CompanyCreatedEventArgs(ChatCommand chatCommand) 
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
        CompanyName = chatCommand.ArgumentsAsString;
    }
}