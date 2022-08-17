using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.CustomEventArgs;

public class ChangeCompanyNameEventArgs : BaseCustomEventArgs
{
    public string CompanyName { get; }

    public ChangeCompanyNameEventArgs(ChatCommand chatCommand)
        : base(chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName)
    {
        CompanyName = chatCommand.ArgumentsAsString;
    }
}