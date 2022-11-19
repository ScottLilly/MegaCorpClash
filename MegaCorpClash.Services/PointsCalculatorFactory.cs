using MegaCorpClash.Models;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services;

public class PointsCalculatorFactory
{
    private static readonly NLog.Logger Logger =
        NLog.LogManager.GetCurrentClassLogger();

    private static object s_syncLock = new();

    private static readonly HashSet<string> s_chattersSinceLastTick = new();
    private static int s_bonusPointsNextTurn = 0;
    private static int s_streamMultiplier = 1;

    private readonly GameSettings.PointsInfo _pointsInfo;
    private readonly IRepository _companyRepository;

    public PointsCalculatorFactory(GameSettings.PointsInfo pointsInfo, 
        IRepository companyRepository)
    {
        _pointsInfo = pointsInfo;
        _companyRepository = companyRepository;
    }

    public PointsCalculator GetPointsCalculator()
    {
        lock (s_syncLock)
        {
            var pointsCalculator = new PointsCalculator(_pointsInfo, _companyRepository);

            pointsCalculator.SetBonusPointsForNextTurn(s_bonusPointsNextTurn);
            s_bonusPointsNextTurn = 0;
            pointsCalculator.SetStreamMultiplier(s_streamMultiplier);
            foreach(var chatterId in s_chattersSinceLastTick)
            {
                pointsCalculator.RecordPlayerChatted(chatterId);
            }
            s_chattersSinceLastTick.Clear();

            return pointsCalculator;
        }
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
}
