using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class RenameCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    public string CommandText => "rename";

    public RenameCommandHandler(Dictionary<string, Player> players) 
        : base(players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        //if (chatCommand.ArgumentsAsList.Any())
        //{
        //    OnCompanyNameChanged?.Invoke(this, new ChangeCompanyNameEventArgs(chatCommand));
        //}
        //else
        //{
        //    OnMessageToLog?
        //        .Invoke(this,
        //            new MessageEventArgs(
        //                chatCommand,
        //                $"{chatCommand.ChatMessage.DisplayName} - !rename must be followed by the new name for your company",
        //                true));
        //}
    }
}