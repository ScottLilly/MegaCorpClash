using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class RenameCommandHandler : IHandleChatCommand
{
    public string CommandText => "rename";

    public event EventHandler<ChangeCompanyNameEventArgs> OnCompanyNameChanged;
    public event EventHandler<MessageEventArgs> OnMessageToLog;

    public void Execute(ChatCommand chatCommand)
    {
        if (chatCommand.ArgumentsAsList.Any())
        {
            OnCompanyNameChanged?.Invoke(this, new ChangeCompanyNameEventArgs(chatCommand));
        }
        else
        {
            OnMessageToLog?
                .Invoke(this,
                    new MessageEventArgs(
                        chatCommand,
                        $"{chatCommand.ChatMessage.DisplayName} - !rename must be followed by the new name for your company",
                        true));
        }
    }
}