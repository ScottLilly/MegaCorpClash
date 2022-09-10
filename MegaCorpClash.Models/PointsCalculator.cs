namespace MegaCorpClash.Models;

public class PointsCalculator
{
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Company> _players;

    private static readonly HashSet<string> s_chattersDuringTurn = new();
    private static readonly object s_syncLock = new();
    private static int s_bonusPointsNextTurn = 0;

    public PointsCalculator(GameSettings gameSettings, 
        Dictionary<string, Company> players)
    {
        _gameSettings = gameSettings;
        _players = players;
    }

    public void SetBonusPointsForNextTurn(int bonusPoints)
    {
        lock (s_syncLock)
        {
            s_bonusPointsNextTurn = bonusPoints;
        }
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
                player.Points += s_bonusPointsNextTurn;

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
            s_bonusPointsNextTurn = 0;
        }
    }
}