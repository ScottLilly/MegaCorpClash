using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;

namespace MegaCorpClash.Services;

public class CommandHandlerFactory
{
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Company> _companies;
    private readonly Dictionary<string, Type> _commandHandlerTypes = new();

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
            if (Activator.CreateInstance(commandHandlerType, _gameSettings, _companies) is BaseCommandHandler instance)
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