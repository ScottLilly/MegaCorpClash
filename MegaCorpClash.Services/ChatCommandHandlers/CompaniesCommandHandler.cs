using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class CompaniesCommandHandler : BaseCommandHandler
{
    public CompaniesCommandHandler(GameSettings gameSettings,
        IRepository companyRepository)
        : base("companies", gameSettings, companyRepository)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

        PublishMessage(CompanyRepository.GetAllCompanies().None()
            ? Literals.Companies_NoCompaniesInGame
            : $"Richest companies: {TopCompaniesByPoints}");
    }
}