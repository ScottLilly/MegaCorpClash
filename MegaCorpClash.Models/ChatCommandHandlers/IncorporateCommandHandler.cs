﻿using CSharpExtender.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class IncorporateCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    public string CommandName => "incorporate";

    public IncorporateCommandHandler(Dictionary<string, Player> players)
        : base(players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        string chatterDisplayName = DisplayName(chatCommand);
        string? companyName = chatCommand.ArgumentsAsString;

        if (string.IsNullOrWhiteSpace(companyName))
        {
            PublishMessage(chatterDisplayName, 
                "!incorporate must be followed by a name for your company");

            return;
        }

        Player? player = GetPlayerObjectForChatter(chatCommand);

        if (player != null)
        {
            PublishMessage(chatterDisplayName,
                $"You already have a company named {player.CompanyName}");

            return;
        }

        if (_players.Values.Any(p => p.CompanyName.Matches(companyName)))
        {
            PublishMessage(chatterDisplayName,
                $"There is already a company named {companyName}");

            return;
        }

        string twitchUserId = TwitchUserId(chatCommand);

        player = new Player
        {
            Id = twitchUserId,
            DisplayName = chatterDisplayName,
            CompanyName = companyName,
            CreatedOn = DateTime.UtcNow
        };

        _players[twitchUserId] = player;

        NotifyPlayerDataUpdated();

        PublishMessage(chatterDisplayName,
            $"You are now the proud CEO of {companyName}");
    }
}