namespace MegaCorpClash.Models;

public class GameSettings
{
    public string ChannelName { get; set; }
    public string BotAccountName { get; set; }
    public string TwitchToken { get; set; }
    public string BotDisplayName { get; set; }
    public TimedMessageSettings TimedMessages { get; set; }

    public class TimedMessageSettings
    {
        public int IntervalInMinutes { get; set; }
        public List<string> Messages { get; set; }
    }
}