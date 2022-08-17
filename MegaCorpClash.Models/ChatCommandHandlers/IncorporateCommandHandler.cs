using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class IncorporateCommandHandler : IHandleChatCommand
{
    public string CommandText => "incorporate";

    public event EventHandler<CreateCompanyEventArgs>? OnCompanyCreated;
    public event EventHandler<MessageEventArgs> OnMessageToLog;

    public void Execute(ChatCommand chatCommand)
    {
        if (chatCommand.ArgumentsAsList.Any())
        {
            OnCompanyCreated?
                .Invoke(this, new CreateCompanyEventArgs(chatCommand));
        }
        else
        {
            OnMessageToLog?
                .Invoke(this, 
                    new MessageEventArgs(
                        chatCommand,
                        $"{chatCommand.ChatMessage.DisplayName} - !incorporate must be followed by a name for your company", 
                        true));
        }
    }
}