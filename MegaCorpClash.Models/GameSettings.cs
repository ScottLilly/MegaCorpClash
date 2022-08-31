namespace MegaCorpClash.Models;

public class GameSettings
{
    public string ChannelName { get; set; }
    public string BotAccountName { get; set; }
    public string TwitchToken { get; set; }
    public string PointsName { get; set; }
    public TurnInfo TurnDetails { get; set; }
    public TimedMessageSettings TimedMessages { get; set; }
    public  List<EmployeeHiringDetails> EmployeeHiringDetails { get; set; }

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
}