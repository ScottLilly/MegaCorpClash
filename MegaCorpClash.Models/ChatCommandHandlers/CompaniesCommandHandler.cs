﻿using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class CompaniesCommandHandler : BaseCommandHandler
{
    public CompaniesCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Player> players)
        : base("companies", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        string playerList =
            string.Join(", ",
                Players.Values
                    .OrderBy(c => c.CompanyName)
                    .Select(c => $"{c.CompanyName} ({c.DisplayName})"));

        PublishMessage($"Companies: {playerList}");
    }
}