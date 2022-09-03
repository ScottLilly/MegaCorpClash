﻿using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestSetMottoCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new SetMottoCommandHandler(_gameSettings, companies);

        var chatCommand = GetChatCommand("!setmotto");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(chatCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.YouDoNotHaveACompany,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_HasCompanyMissingMottoParameter()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterId = DEFAULT_CHATTER_ID,
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new SetMottoCommandHandler(_gameSettings, companies);

        var chatCommand = GetChatCommand("!setmotto");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(chatCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("You must enter a value for the motto",
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_Success_DefaultMotto()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterId = DEFAULT_CHATTER_ID,
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Equal("We don't need a motto", company.Motto);
    }

    [Fact]
    public void Test_Success_ModifiedMotto()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterId = DEFAULT_CHATTER_ID,
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 99
            });

        var commandHandler =
            new SetMottoCommandHandler(_gameSettings, companies);

        var chatCommand = GetChatCommand("!setmotto This is our new motto");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(chatCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("Your new company motto is 'This is our new motto'",
            chatMessageEvent.Arguments.Message);
    }
}