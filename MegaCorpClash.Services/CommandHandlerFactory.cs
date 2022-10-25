using MegaCorpClash.Models;
using MegaCorpClash.Models.BroadcasterCommandHandlers;
using MegaCorpClash.Models.ChatCommandHandlers;

namespace MegaCorpClash.Services;

public class CommandHandlerFactory
{
    private readonly Dictionary<string, Type> _commandHandlerTypes = new();

    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Company> _companies;

    public CommandHandlerFactory(GameSettings gameSettings,
        Dictionary<string, Company> companies)
    {
        _gameSettings = gameSettings;
        _companies = companies;

        var baseType = typeof(BaseCommandHandler);
        var assembly = baseType.Assembly;

        var commandHandlerTypes = 
            assembly.GetTypes()
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);

        foreach (var commandHandlerType in commandHandlerTypes)
        {
            var instance =
                Activator.CreateInstance(commandHandlerType, _gameSettings, _companies) as BaseCommandHandler;

            if (instance != null)
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
                Activator.CreateInstance(commandHandlerType, _gameSettings, _companies);

            return instance as BaseCommandHandler;
        }

        return null;
    }
}