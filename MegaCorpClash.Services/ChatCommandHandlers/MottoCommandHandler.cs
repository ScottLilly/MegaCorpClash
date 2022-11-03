﻿using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class MottoCommandHandler : BaseCommandHandler
{
    public MottoCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("motto", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

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

        NotifyCompanyDataUpdated();
        PublishMessage($"Your new company motto is '{chatter.Company.Motto}'");
    }
}