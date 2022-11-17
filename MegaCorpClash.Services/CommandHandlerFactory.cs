using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
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
                _gameSettings, _companyRepository) is BaseCommandHandler instance)
            {
                _commandHandlerTypes.Add(instance.CommandName, commandHandlerType);
            }
        }

    }

    public BaseCommandHandler? GetCommandHandlerForCommand(string command)
    {
        if (_commandHandlerTypes.ContainsKey(command))
        {
            var commandHandlerType = _commandHandlerTypes[command];

            var instance =
                Activator.CreateInstance(commandHandlerType, 
                _gameSettings, _companyRepository);

            return instance as BaseCommandHandler;
        }

        return null;
    }
}