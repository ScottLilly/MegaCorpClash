using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class CompaniesCommandHandler : BaseCommandHandler
{
    public CompaniesCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("companies", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        PublishMessage(Companies.None()
            ? Literals.Companies_NoCompaniesInGame
            : $"Richest companies: {TopCompaniesByPoints}");
    }
}