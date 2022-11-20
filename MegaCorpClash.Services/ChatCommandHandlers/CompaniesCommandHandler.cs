using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class CompaniesCommandHandler : BaseCommandHandler
{
    public CompaniesCommandHandler(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs)
        : base("companies", gameSettings, companyCompanyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        PublishMessage(CompanyCompanyRepository.GetAllCompanies().None()
            ? Literals.Companies_NoCompaniesInGame
            : $"Richest companies: {TopCompaniesByPoints}");
    }
}