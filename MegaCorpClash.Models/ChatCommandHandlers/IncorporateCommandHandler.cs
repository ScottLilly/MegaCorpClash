﻿using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class IncorporateCommandHandler : BaseCommandHandler
{
    public IncorporateCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("incorporate", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);

        if (gameCommand.DoesNotHaveArguments)
        {
            PublishMessage(chatter.ChatterName, 
                Literals.Incorporate_NameRequired);
            return;
        }

        if (gameCommand.Argument.IsNotSafeText())
        {
            PublishMessage(chatter.ChatterName,
                Literals.Incorporate_NotSafeText);
            return;
        }

        if (gameCommand.Argument.Length > 
            GameSettings.MaxCompanyNameLength)
        {
            PublishMessage(chatter.ChatterName,
                $"Company name cannot be longer than {GameSettings.MaxCompanyNameLength} characters");
            return;
        }

        if (chatter.Company != null)
        {
            PublishMessage(chatter.ChatterName,
                $"You already have a company named {chatter.Company.CompanyName}");
            return;
        }

        if (Companies.Values.Any(p => p.CompanyName.Matches(gameCommand.Argument)))
        {
            PublishMessage(chatter.ChatterName,
                $"There is already a company named {gameCommand.Argument}");
            return;
        }

        chatter.Company = 
            new Company
            {
                UserId = chatter.ChatterId,
                DisplayName = chatter.ChatterName,
                IsBroadcaster = 
                    chatter.ChatterName.Matches(GameSettings.TwitchBroadcasterAccount.Name),
                CompanyName = gameCommand.Argument,
                CreatedOn = DateTime.UtcNow,
                Points = GameSettings?.StartupDetails.InitialPoints ?? 0
            };

        GiveDefaultStaff(chatter.Company);

        Companies[chatter.ChatterId] = chatter.Company;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.ChatterName,
            $"You are now the proud CEO of {gameCommand.Argument}");
    }

    private void GiveDefaultStaff(Company company)
    {
        foreach (var staffDetails in GameSettings.StartupDetails.InitialStaff)
        {
            company.Employees
                .Add(new EmployeeQuantity
                {
                    Type = staffDetails.Type,
                    Quantity = staffDetails.Quantity
                });
        }
    }
}