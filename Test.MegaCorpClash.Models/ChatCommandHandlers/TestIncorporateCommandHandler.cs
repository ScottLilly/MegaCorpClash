using System.Drawing;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models;
using TwitchLib.Client.Models;
using Moq;
using TwitchLib.Client.Enums;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestIncorporateCommandHandler
{
    [Fact]
    public void Test_Instantiate()
    {
        GameSettings gameSettings = 
            new GameSettings();
        Dictionary<string, Player> playerList = 
            new Dictionary<string, Player>();

        var incorporateCommandHandler = 
            new IncorporateCommandHandler(gameSettings, playerList);

        Assert.NotNull(incorporateCommandHandler);
    }

    [Fact]
    public void Test_NoCompanyNamePassed()
    {
        GameSettings gameSettings =
            new GameSettings();
        Dictionary<string, Player> playerList =
            new Dictionary<string, Player>();

        var incorporateCommandHandler =
            new IncorporateCommandHandler(gameSettings, playerList);

        var chatCommand = 
            GetChatCommand("123", "CodingWithScott", "!incorporate");

        var evt = Assert.Raises<ChatMessageEventArgs>(
            h => incorporateCommandHandler.OnChatMessagePublished += h,
            h => incorporateCommandHandler.OnChatMessagePublished -= h,
            () => incorporateCommandHandler.Execute(chatCommand));

        Assert.NotNull(evt);
        Assert.Equal("CodingWithScott", evt.Arguments.ChatterDisplayName);
        Assert.Equal("!incorporate must be followed by a name for your company", evt.Arguments.Message);

        Assert.NotNull(incorporateCommandHandler);
    }

    private ChatCommand GetChatCommand(string userId, string displayName, 
        string commandText)
    {
        var chatMessage =
            new ChatMessage("CodingWithScottBot", userId, "codingwithscott",
                displayName, "", Color.AliceBlue, null, "message goes here",
                UserType.Viewer, "codingwithscott", "456", false, 0,
                "789", false, false, false, false, false, false, false, Noisy.False,
                "", "", new List<KeyValuePair<string, string>>(), new CheerBadge(0),
                0, 0d);

        if (commandText[0] == '!')
        {
            commandText = commandText.Substring(1);
        }

        var commandWords = 
            commandText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return 
            new ChatCommand(chatMessage,
                commandWords[0], 
                string.Join(' ', commandWords.Skip(1)),
                commandWords.Skip(1).ToList(),
                '!');

    }
}