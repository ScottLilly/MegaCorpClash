using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class CompaniesCommandHandler : BaseCommandHandler
{
    public CompaniesCommandHandler(GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base("companies", gameSettings, companyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        PublishMessage(CompanyRepository.GetAllCompanies().None()
            ? Literals.Companies_NoCompaniesInGame
            : $"Richest companies: {TopCompaniesByPoints}");
    }
}