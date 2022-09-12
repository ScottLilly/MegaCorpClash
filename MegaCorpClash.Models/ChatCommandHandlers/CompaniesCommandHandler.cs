using CSharpExtender.ExtensionMethods;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class CompaniesCommandHandler : BaseCommandHandler
{
    public CompaniesCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("companies", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
    {
        PublishMessage(Companies.None()
            ? Literals.Companies_NoCompaniesInGame
            : $"Companies: {CompaniesList}");
    }
}