namespace MegaCorpClash.Models;

public class PointsCalculator
{
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Company> _companies;

    private static readonly HashSet<string> s_chattersSinceStartup = new();
    private static readonly HashSet<string> s_chattersDuringTurn = new();
    private static readonly object s_syncLock = new();
    private static int s_bonusPointsNextTurn = 0;

    public PointsCalculator(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
    {
        _gameSettings = gameSettings;
        _companies = companies;
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
            s_chattersSinceStartup.Add(userId);
        }
    }

    public void ApplyPointsForTurn()
    {
        lock (s_syncLock)
        {
            foreach (var company in _companies.Values)
            {
                // Get base points
                int pointsForTurn = 0;

                if (company.IsBroadcaster)
                {
                    pointsForTurn =
                        _gameSettings.TurnDetails.PointsPerTurn.Chatter * 5;
                }
                else if (s_chattersDuringTurn.Contains(company.ChatterId))
                {
                    pointsForTurn = 
                        _gameSettings.TurnDetails.PointsPerTurn.Chatter;
                }
                else
                {
                    pointsForTurn = 
                        _gameSettings.TurnDetails.PointsPerTurn.Lurker;
                }

                // Apply multiplier


                // Apply bonus
                if (s_chattersSinceStartup.Contains(company.ChatterId))
                {
                    pointsForTurn += s_bonusPointsNextTurn;
                }

                // Add points to player
                company.Points += pointsForTurn;
            }

            s_chattersDuringTurn.Clear();
            s_bonusPointsNextTurn = 0;
        }
    }
}