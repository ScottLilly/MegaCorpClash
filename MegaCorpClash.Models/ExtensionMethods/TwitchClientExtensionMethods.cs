using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ExtensionMethods;

internal static class TwitchClientExtensionMethods
{
    internal static string ChatterUserId(this ChatCommand chatCommand) =>
        chatCommand.ChatMessage.UserId;
    internal static string ChatterDisplayName(this ChatCommand chatCommand) =>
        chatCommand.ChatMessage.DisplayName;
}