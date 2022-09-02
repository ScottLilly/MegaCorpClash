using MegaCorpClash.Models;
using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandlerTest
{
    private const char COMMAND_PREFIX = '!';

    protected const string POINTS_NAME = "CorpoBux";
    protected const string DEFAULT_CHATTER_ID = "123";
    protected const string DEFAULT_CHATTER_DISPLAY_NAME = "CodingWithScott";

    internal static GameSettings GetDefaultGameSettings()
    {
        return new GameSettings
        {
            PointsName = POINTS_NAME
        };
    }

    internal static ChatCommand GetChatCommand(string command)
    {
        return GetChatCommand(DEFAULT_CHATTER_ID,
            DEFAULT_CHATTER_DISPLAY_NAME, command);
    }

    private static ChatCommand GetChatCommand(string userId,
        string displayName, string commandText)
    {
        var chatMessage =
            new ChatMessage("CodingWithScottBot", userId, "codingwithscott",
                displayName, "", Color.AliceBlue, null, "message goes here",
                UserType.Viewer, "codingwithscott", "456", false, 0,
                "789", false, false, false, false, false, false, false, Noisy.False,
                "", "", new List<KeyValuePair<string, string>>(), new CheerBadge(0),
                0, 0d);

        if (commandText[0] == COMMAND_PREFIX)
        {
            commandText = commandText[1..];
        }

        var commandWords =
            commandText.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return
            new ChatCommand(chatMessage,
                commandWords[0],
                string.Join(' ', commandWords.Skip(1)),
                commandWords.Skip(1).ToList(),
                COMMAND_PREFIX);
    }
}