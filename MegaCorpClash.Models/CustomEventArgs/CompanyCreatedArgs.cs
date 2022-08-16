using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class CompanyCreatedArgs : BaseCustomEventArgs
{
    public string CompanyName { get; }

    public CompanyCreatedArgs(ChatCommand chatCommand) 
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
        CompanyName = chatCommand.ArgumentsAsString;
    }
}