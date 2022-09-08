namespace MegaCorpClash.Models;

public class PointsCalculator
{
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Company> _players;

    private static readonly HashSet<string> s_chattersDuringTurn = new();
    private static readonly object s_syncLock = new();

    public PointsCalculator(GameSettings gameSettings, 
        Dictionary<string, Company> players)
    {
        _gameSettings = gameSettings;
        _players = players;
    }

    public void RecordPlayerChatted(string userId)
    {
        lock (s_syncLock)
        {
            s_chattersDuringTurn.Add(userId);
        }
    }

    public void ApplyPointsForTurn()
    {
        lock (s_syncLock)
        {
            foreach (var player in _players.Values)
            {
                if (player.IsBroadcaster)
                {
                    player.Points +=
                        _gameSettings.TurnDetails.PointsPerTurn.Chatter * 5;
                }
                else if (s_chattersDuringTurn.Contains(player.ChatterId))
                {
                    player.Points += 
                        _gameSettings.TurnDetails.PointsPerTurn.Chatter;
                }
                else
                {
                    player.Points += 
                        _gameSettings.TurnDetails.PointsPerTurn.Lurker;
                }
            }

            s_chattersDuringTurn.Clear();
        }
    }
}