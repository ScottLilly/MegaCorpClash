using MegaCorpClash.Models;
using MegaCorpClash.Services.Persistence;
using MegaCorpClash.Services.Queues;

namespace MegaCorpClash.Services;

public class PointsCalculator : IExecutable
{
    private static readonly NLog.Logger Logger = 
        NLog.LogManager.GetCurrentClassLogger();

    private readonly GameSettings.PointsInfo _pointsInfo;
    private readonly ICompanyRepository _companyCompanyRepository;

    private static readonly HashSet<string> s_chattersSinceLastTick = new();
    private static readonly object s_syncLock = new();
    private static int s_bonusPointsNextTurn = 0;
    private static int s_streamMultiplier = 1;

    public PointsCalculator(GameSettings.PointsInfo pointsInfo,
        ICompanyRepository companyCompanyRepository)
    {
        _pointsInfo = pointsInfo;
        _companyCompanyRepository = companyCompanyRepository;
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
            s_chattersSinceLastTick.Add(userId);
        }
    }

    public void Execute()
    {
        lock (s_syncLock)
        {
            foreach (var company in _companyCompanyRepository.GetAllCompanies())
            {
                // Get base points
                int pointsForTurn = 0;

                if (company.IsBroadcaster)
                {
                    pointsForTurn = _pointsInfo.Chatter * 5;
                }
                else if (s_chattersSinceLastTick.Contains(company.UserId))
                {
                    pointsForTurn = _pointsInfo.Chatter;
                }
                else
                {
                    pointsForTurn = _pointsInfo.Lurker;
                }

                if(company.IsSubscriber)
                {
                    pointsForTurn *= 
                        Math.Max(1, _pointsInfo.SubscriberMultiplier);
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
                pointsForTurn += s_bonusPointsNextTurn;

                // Apply stream multiplier
                pointsForTurn *= s_streamMultiplier;

                // Add points to player
                _companyCompanyRepository.AddPoints(company.UserId, pointsForTurn);
            }

            s_bonusPointsNextTurn = 0;
        }
    }
}