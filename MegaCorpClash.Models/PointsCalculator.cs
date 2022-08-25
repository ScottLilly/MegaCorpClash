namespace MegaCorpClash.Models;

public class PointsCalculator
{
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Player> _players;

    private static readonly HashSet<string> s_chattersDuringTurn = new();
    private static readonly object s_syncLock = new();

    public PointsCalculator(GameSettings gameSettings, 
        Dictionary<string, Player> players)
    {
        _gameSettings = gameSettings;
        _players = players;
    }

    public void RecordChatTimeForPlayer(string userId)
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
                if (s_chattersDuringTurn.Contains(player.Id))
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