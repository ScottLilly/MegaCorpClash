﻿using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services;

public class CommandHandlerFactory
{
    private readonly GameSettings _gameSettings;
    private readonly IRepository _companyRepository;
    private readonly Dictionary<string, Type> _commandHandlerTypes = new();

    public CommandHandlerFactory(GameSettings gameSettings,
        IRepository companyRepository)
    {
        _gameSettings = gameSettings;
        _companyRepository = companyRepository;

        var baseType = typeof(BaseCommandHandler);
        var assembly = baseType.Assembly;

        var commandHandlerTypes =
            assembly.GetTypes()
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);

        foreach (var commandHandlerType in commandHandlerTypes)
        {
            if (Activator.CreateInstance(commandHandlerType, 
                _gameSettings, _companyRepository, null) is BaseCommandHandler instance)
            {
                _commandHandlerTypes.Add(instance.CommandName.ToLowerInvariant(), commandHandlerType);
            }
        }

    }

    public BaseCommandHandler? GetCommandHandlerForCommand(GameCommandArgs gameCommandArgs)
    {
        if (_commandHandlerTypes.ContainsKey(gameCommandArgs.CommandName.ToLowerInvariant()))
        {
            var commandHandlerType = _commandHandlerTypes[gameCommandArgs.CommandName.ToLowerInvariant()];

            var instance =
                Activator.CreateInstance(commandHandlerType, 
                _gameSettings, _companyRepository, gameCommandArgs);

            return instance as BaseCommandHandler;
        }

        return null;
    }
}