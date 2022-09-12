using CSharpExtender.ExtensionMethods;
using Newtonsoft.Json.Converters;

namespace MegaCorpClash.Models;

public sealed class GameSettings
{
    public List<TwitchAccount> TwitchAccounts { get; set; }
    public string PointsName { get; set; }
    public TurnInfo TurnDetails { get; set; }
    public StartupInfo StartupDetails { get; set; }
    public TimedMessageSettings TimedMessages { get; set; }
    public List<EmployeeHiringInfo> EmployeeHiringDetails { get; set; } = new();

    public TwitchAccount? TwitchBroadcasterAccount =>
        TwitchAccounts.FirstOrDefault(ta => ta.Type.Matches("Broadcaster"));
    public TwitchAccount? TwitchBotAccount =>
        TwitchAccounts.FirstOrDefault(ta => ta.Type.Matches("Bot"));

    public string JobsList =>
        string.Join(", ",
            EmployeeHiringDetails
                .OrderBy(e => e.Type)
                .Select(e => $"{e.Type} ({e.CostToHire})"));

    public class TwitchAccount
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string AuthToken { get; set; }
    }

    public class TimedMessageSettings
    {
        public int IntervalInMinutes { get; set; }
        public List<string> Messages { get; set; }
    }

    public class TurnInfo
    {
        public int MinutesPerTurn { get; set; }
        public PointsInfo PointsPerTurn { get; set; }
    }

    public class PointsInfo
    {
        public int Lurker { get; set; }
        public int Chatter { get; set; }
    }

    public class StartupInfo
    {
        public int InitialPoints { get; set; }
        public List<StaffDetails> InitialStaff { get; set; }

        public class StaffDetails
        {
            [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
            public EmployeeType Type { get; set; }
            public int Qty { get; set; }
        }
    }

    public class EmployeeHiringInfo
    {
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public EmployeeType Type { get; set; }
        public int CostToHire { get; set; }
    }
}