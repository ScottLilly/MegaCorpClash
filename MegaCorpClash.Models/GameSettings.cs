using Newtonsoft.Json.Converters;

namespace MegaCorpClash.Models;

public class GameSettings
{
    public string ChannelName { get; set; }
    public string BotAccountName { get; set; }
    public string TwitchToken { get; set; }
    public string PointsName { get; set; }
    public TurnInfo TurnDetails { get; set; }
    public StartupInfo StartupDetails { get; set; }
    public TimedMessageSettings TimedMessages { get; set; }
    public List<EmployeeHiringInfo> EmployeeHiringDetails { get; set; } = new();

    public string JobsList =>
        string.Join(", ",
            EmployeeHiringDetails
                .OrderBy(e => e.Type)
                .Select(e => $"{e.Type} ({e.CostToHire})"));

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