﻿using CSharpExtender.ExtensionMethods;
using CSharpExtender.Services;
using LiteDB;
using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;
using MegaCorpClash.Services.Queues;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public abstract class BaseCommandHandler : IExecutable
{
    protected static readonly NLog.Logger Logger =
        NLog.LogManager.GetCurrentClassLogger();
   
    protected readonly ArgumentParser _argumentParser = new();

    public string CommandName { get; }
    public bool BroadcasterCanRun { get; protected init; } = true;
    public bool NonBroadcasterCanRun { get; protected init; } = true;
    protected GameSettings GameSettings { get; }
    protected IRepository CompanyRepository { get; }
    protected GameCommandArgs GameCommandArgs { get; }

    protected string TopCompaniesByPoints =>
        string.Join(", ",
            CompanyRepository.GetRichestCompanies(7)
                .Select(c => $"{c.CompanyName} [{c.Points:N0}]"));

    public List<string> ChatMessages { get; } = new();
    public bool StreamerBankrupted { get; private set; } = false;

    public ChatterDetails
        ChatterDetails() =>
        new ChatterDetails(GameCommandArgs.UserId,
            GameCommandArgs.DisplayName,
            CompanyRepository.GetCompany(GameCommandArgs.UserId));

    protected Company GetBroadcasterCompany =>
        CompanyRepository.GetBroadcasterCompany();

    protected BaseCommandHandler(string commandName, GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
    {
        CommandName = commandName;
        GameSettings = gameSettings;
        CompanyRepository = companyRepository;
        GameCommandArgs = gameCommandArgs;
    }

    public abstract void Execute();

    protected bool IsAttackSuccessful(EmployeeType guardEmployeeType)
    {
        var broadcasterCompany = GetBroadcasterCompany;

        int securityEmployeeCount =
            broadcasterCompany.Employees
                .Where(e => e.Type == guardEmployeeType)
                .Sum(e => e.Quantity)
            + 1;

        int broadcasterDefenseBonus =
            Math.Min(25, Convert.ToInt32(Math.Log10(securityEmployeeCount) * 10));

        int rand = RngCreator.GetNumberBetween(1, 100);

        return rand > 50 + broadcasterDefenseBonus;
    }

    protected int GetNumberOfAttackingSpies(GameCommandArgs gameCommand, Company company)
    {
        int numberOfAttackingSpies = 1;

        var parsedArguments =
            _argumentParser.Parse(gameCommand.Argument);

        if (parsedArguments.IntegerArguments.Count == 1)
        {
            numberOfAttackingSpies = parsedArguments.IntegerArguments.First();
        }
        else if (parsedArguments.StringArguments.Any(s => s.Matches("max")))
        {
            numberOfAttackingSpies =
                company.Employees
                    .FirstOrDefault(e => e.Type == EmployeeType.Spy)?.Quantity ?? 0;
        }

        return numberOfAttackingSpies;
    }

    protected void SetMessageForInsufficientSpies(int availableSpies)
    {
        if (availableSpies == 0)
        {
            PublishMessage("You don't have any spies");
        }
        else if (availableSpies == 1)
        {
            PublishMessage("You only have 1 spy");
        }
        else
        {
            PublishMessage($"You only have {availableSpies} spies");
        }
    }

    protected void LogTraceMessage()
    {
        Logger.Trace("Begin execution of command: {CommandName}", CommandName);
    }

    protected void PublishMessage(string message)
    {
        ChatMessages.Add(message);
    }

    protected void NotifyBankruptedStreamer()
    {
        StreamerBankrupted = true;
    }
}