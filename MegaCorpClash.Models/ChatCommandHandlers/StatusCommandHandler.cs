﻿using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class StatusCommandHandler : BaseCommandHandler
{
    public StatusCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("status", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);

        PublishMessage(chatter.ChatterName,
            chatter.Company == null
                ? Literals.YouDoNotHaveACompany
                : $"At {chatter.Company.CompanyName} we always say '{chatter.Company.Motto}'. That's how we earned {chatter.Company.Points:N0} {GameSettings.PointsName}");
    }
}