using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.BroadcasterCommandHandlers;

public abstract class BroadcasterOnlyCommandHandler : BaseCommandHandler
{
    public event EventHandler<BroadcasterCommandEventArgs> OnBroadcasterCommand;

    protected BroadcasterOnlyCommandHandler(
        string commandName, GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base(commandName, gameSettings, companyRepository, gameCommandArgs)
    {
    }

    protected void RaiseBroadcasterCommandEvent(BroadcasterCommandEventArgs e)
    {
        OnBroadcasterCommand?.Invoke(this, e);
    }
}