using MegaCorpClash.Models;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services;

public class PointsCalculator
{
    private static readonly NLog.Logger Logger = 
        NLog.LogManager.GetCurrentClassLogger();

    private readonly GameSettings _gameSettings;
    private readonly IRepository _companyRepository;

    private static readonly HashSet<string> s_chattersSinceStartup = new();
    private static readonly HashSet<string> s_chattersDuringTurn = new();
    private static readonly object s_syncLock = new();
    private static int s_bonusPointsNextTurn = 0;
    private static int s_streamMultiplier = 1;

    public PointsCalculator(GameSettings gameSettings,
        IRepository companyRepository)
    {
        _gameSettings = gameSettings;
        _companyRepository = companyRepository;
    }

    public void SetBonusPointsForNextTurn(int bonusPoints)
    {
        if (bonusPoints < 0)
        {
            return;
        }

        lock (s_syncLock)
        {
            s_bonusPointsNextTurn = bonusPoints;
            Logger.Trace($"Set bonus points to {bonusPoints}");
        }
    }

    public void SetStreamMultiplier(int multiplier)
    {
        if (multiplier <= 0)
        {
            return;
        }

        lock (s_syncLock)
        {
            s_streamMultiplier = multiplier;
            Logger.Trace($"Set stream multiplier to {multiplier}");
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
            foreach (var company in _companyRepository.GetAllCompanies())
            {
                // Get base points
                int pointsForTurn = 0;

                if (company.IsBroadcaster)
                {
                    pointsForTurn =
                        _gameSettings.TurnDetails.PointsPerTurn.Chatter * 5;
                }
                else if (s_chattersDuringTurn.Contains(company.UserId))
                {
                    pointsForTurn =
                        _gameSettings.TurnDetails.PointsPerTurn.Chatter;
                }
                else
                {
                    pointsForTurn =
                        _gameSettings.TurnDetails.PointsPerTurn.Lurker;
                }

                if(company.IsSubscriber)
                {
                    pointsForTurn *= 
                        Math.Max(1, _gameSettings.TurnDetails.PointsPerTurn.SubscriberMultiplier);
                }

                // Apply employee multipliers
                int salesPeople =
                    company.Employees.
                        FirstOrDefault(e => e.Type == EmployeeType.Sales)?.Quantity ?? 1;

                pointsForTurn *=
                    Math.Max(1,
                    Convert.ToInt32(Math.Round(Convert.ToDecimal(salesPeople) / 5M,
                    MidpointRounding.AwayFromZero)));

                // Apply one-time tick interval bonus
                if (s_chattersSinceStartup.Contains(company.UserId))
                {
                    pointsForTurn += s_bonusPointsNextTurn;
                }

                // Apply stream multiplier
                pointsForTurn *= s_streamMultiplier;

                // Add points to player
                _companyRepository.AddPoints(company.UserId, pointsForTurn);
            }

            s_chattersDuringTurn.Clear();
            s_bonusPointsNextTurn = 0;
        }
    }
}