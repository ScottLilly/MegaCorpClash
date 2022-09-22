using MegaCorpClash.Models;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandlerTest
{
    private const char COMMAND_PREFIX = '!';

    protected const string POINTS_NAME = "CorpoBux";
    protected const string DEFAULT_CHATTER_ID = "123";
    protected const string DEFAULT_CHATTER_DISPLAY_NAME = "CodingWithScott";

    internal static GameSettings GetDefaultGameSettings()
    {
        var gameSettings = new GameSettings
        {
            TwitchAccounts = new List<GameSettings.TwitchAccount>()
            {
                new()
                {
                    Type = "Broadcaster",
                    Name = "BroadcasterAccount",
                    AuthToken = "123"
                },
                new()
                {
                    Type = "Bot",
                    Name = "BotAccount",
                    AuthToken = "123"
                }
            },
            PointsName = POINTS_NAME,
            MaxCompanyNameLength = 15,
            MaxMottoLength = 25,
            StartupDetails = new GameSettings.StartupInfo
            {
                InitialPoints = 50,
                InitialStaff = new List<GameSettings.StartupInfo.StaffDetails>
                {
                    new()
                    {
                        Type = EmployeeType.Production,
                        Quantity = 1
                    },
                    new()
                    {
                        Type = EmployeeType.Sales,
                        Quantity = 1
                    }
                }
            }
        };

        return gameSettings;
    }

    internal static GameCommandArgs GetGameCommand(string command)
    {
        var commandWords =
            command.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string commandName = string.Empty;

        if (commandWords[0].StartsWith(COMMAND_PREFIX))
        {
            commandName = commandWords[0][1..];
        }

        return new GameCommandArgs(
            DEFAULT_CHATTER_ID,
            DEFAULT_CHATTER_DISPLAY_NAME,
            commandName,
            string.Join(' ', commandWords.Skip(1)),
            false, false, false);
    }
}