using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class CreateCompanyEventArgs : BaseCustomEventArgs
{
    public string CompanyName { get; }

    public CreateCompanyEventArgs(ChatCommand chatCommand) 
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
        CompanyName = chatCommand.ArgumentsAsString;
    }
}