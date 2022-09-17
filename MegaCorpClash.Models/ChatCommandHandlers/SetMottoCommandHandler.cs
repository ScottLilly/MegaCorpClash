using MegaCorpClash.Core;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class SetMottoCommandHandler : BaseCommandHandler
{
    public SetMottoCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("setmotto", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);

        if (chatter.Company == null)
        {
            PublishMessage(chatter.ChatterName, 
                Literals.YouDoNotHaveACompany);
            return;
        }

        if (gameCommand.DoesNotHaveArguments)
        {
            PublishMessage(chatter.ChatterName, 
                Literals.SetMotto_MustProvideMotto);
            return;
        }

        if (gameCommand.Argument.IsNotSafeText())
        {
            PublishMessage(chatter.ChatterName,
                Literals.SetMotto_NotSafeText);
            return;
        }

        if (gameCommand.Argument.Length > 
            GameSettings.MaxMottoLength)
        {
            PublishMessage(chatter.ChatterName,
                $"Motto cannot be longer than {GameSettings.MaxMottoLength} characters");
            return;
        }

        chatter.Company.Motto = gameCommand.Argument;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.ChatterName,
                $"Your new company motto is '{chatter.Company.Motto}'");
    }
}