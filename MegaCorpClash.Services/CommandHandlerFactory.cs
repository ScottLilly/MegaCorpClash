using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services;

public class CommandHandlerFactory
{
    private readonly GameSettings _gameSettings;
    private readonly ICompanyRepository _companyCompanyRepository;
    private readonly Dictionary<string, Type> _commandHandlerTypes = new();

    public CommandHandlerFactory(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository)
    {
        _gameSettings = gameSettings;
        _companyCompanyRepository = companyCompanyRepository;

        var baseType = typeof(BaseCommandHandler);
        var assembly = baseType.Assembly;

        var commandHandlerTypes =
            assembly.GetTypes()
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);

        foreach (var commandHandlerType in commandHandlerTypes)
        {
            if (Activator.CreateInstance(commandHandlerType, 
                _gameSettings, _companyCompanyRepository, null) is BaseCommandHandler instance)
            {
                _commandHandlerTypes.Add(instance.CommandName, commandHandlerType);
            }
        }

    }

    public BaseCommandHandler? GetCommandHandlerForCommand(GameCommandArgs gameCommandArgs)
    {
        if (_commandHandlerTypes.ContainsKey(gameCommandArgs.CommandName))
        {
            var commandHandlerType = _commandHandlerTypes[gameCommandArgs.CommandName];

            var instance =
                Activator.CreateInstance(commandHandlerType, 
                _gameSettings, _companyCompanyRepository, gameCommandArgs);

            return instance as BaseCommandHandler;
        }

        return null;
    }
}