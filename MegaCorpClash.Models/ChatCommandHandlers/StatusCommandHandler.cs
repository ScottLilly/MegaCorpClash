using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class StatusCommandHandler
{
    public string CommandText => "status";

    public event EventHandler<ChatterEventArgs> OnCompanyStatusRequested;

    public void Execute(ChatCommand chatCommand)
    {
        OnCompanyStatusRequested?.Invoke(this, new ChatterEventArgs(chatCommand));
    }
}