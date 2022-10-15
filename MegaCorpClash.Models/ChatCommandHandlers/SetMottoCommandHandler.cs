using MegaCorpClash.Core;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class SetMottoCommandHandler : BaseCommandHandler
{
    public SetMottoCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("setmotto", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        var chatter = ChatterDetails(gameCommandArgs);

        if (chatter.Company == null)
        {
            PublishMessage(Literals.YouDoNotHaveACompany);
            return;
        }

        if (gameCommandArgs.DoesNotHaveArguments)
        {
            PublishMessage(Literals.SetMotto_MustProvideMotto);
            return;
        }

        if (gameCommandArgs.Argument.IsNotSafeText())
        {
            PublishMessage(Literals.SetMotto_NotSafeText);
            return;
        }

        if (gameCommandArgs.Argument.Length > 
            GameSettings.MaxMottoLength)
        {
            PublishMessage($"Motto cannot be longer than {GameSettings.MaxMottoLength} characters");
            return;
        }

        chatter.Company.Motto = gameCommandArgs.Argument;

        NotifyPlayerDataUpdated();
        PublishMessage($"Your new company motto is '{chatter.Company.Motto}'");
    }
}