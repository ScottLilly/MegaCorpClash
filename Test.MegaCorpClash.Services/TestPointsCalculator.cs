using MegaCorpClash.Models;
using MegaCorpClash.Services;

namespace Test.MegaCorpClash.Services;

public class TestPointsCalculator
{
    private const string BROADCASTER_NAME = "TEST";
    private const string CHATTER_1_ID = "111";
    private const string CHATTER_2_ID = "222";
    private const string CHATTER_3_ID = "333";
    private const string CHATTER_4_ID = "444";

    [Fact]
    public void Test_PointsCalculator()
    {
        var gameSettings = GetGameSettings();
        var companies = GetStandardCompanies();

        var pointsCalculator = 
            new PointsCalculator(gameSettings, companies);

        Assert.Equal(0, companies[CHATTER_1_ID].Points);
        Assert.Equal(0, companies[CHATTER_2_ID].Points);
        Assert.Equal(0, companies[CHATTER_3_ID].Points);
        Assert.Equal(0, companies[CHATTER_4_ID].Points);

        pointsCalculator.ApplyPointsForTurn();

        Assert.Equal(50, companies[CHATTER_1_ID].Points);
        Assert.Equal(1, companies[CHATTER_2_ID].Points);
        Assert.Equal(1, companies[CHATTER_3_ID].Points);
        Assert.Equal(1, companies[CHATTER_4_ID].Points);

        pointsCalculator.RecordPlayerChatted(CHATTER_2_ID);
        pointsCalculator.ApplyPointsForTurn();

        Assert.Equal(100, companies[CHATTER_1_ID].Points);
        Assert.Equal(11, companies[CHATTER_2_ID].Points);
        Assert.Equal(2, companies[CHATTER_3_ID].Points);
        Assert.Equal(2, companies[CHATTER_4_ID].Points);

        companies[CHATTER_3_ID].Employees
            .First(e => e.Type == EmployeeType.Sales).Quantity += 9;

        pointsCalculator.ApplyPointsForTurn();

        Assert.Equal(150, companies[CHATTER_1_ID].Points);
        Assert.Equal(12, companies[CHATTER_2_ID].Points);
        Assert.Equal(4, companies[CHATTER_3_ID].Points);
        Assert.Equal(3, companies[CHATTER_4_ID].Points);

        pointsCalculator.SetStreamMultiplier(10);
        pointsCalculator.RecordPlayerChatted(CHATTER_2_ID);

        pointsCalculator.ApplyPointsForTurn();

        Assert.Equal(650, companies[CHATTER_1_ID].Points);
        Assert.Equal(112, companies[CHATTER_2_ID].Points);
        Assert.Equal(24, companies[CHATTER_3_ID].Points);
        Assert.Equal(13, companies[CHATTER_4_ID].Points);
    }

    #region Private support functions

    private static GameSettings GetGameSettings()
    {
        return new GameSettings
        {
            TurnDetails = 
                new GameSettings.TurnInfo
                {
                    MinutesPerTurn = 1,
                    PointsPerTurn = new GameSettings.PointsInfo
                    {
                        Chatter = 10,
                        Lurker = 1
                    }
                }
        };
    }

    private static Dictionary<string, Company> GetStandardCompanies()
    {
        return new Dictionary<string, Company>
        {
            { CHATTER_1_ID, CreateStandardCompany(CHATTER_1_ID, BROADCASTER_NAME) },
            { CHATTER_2_ID, CreateStandardCompany(CHATTER_2_ID, "CHATTER2") },
            { CHATTER_3_ID, CreateStandardCompany(CHATTER_3_ID, "CHATTER3") },
            { CHATTER_4_ID, CreateStandardCompany(CHATTER_4_ID, "CHATTER4") }
        };
    }

    private static Company CreateStandardCompany(string chatterId, string chatterName)
    {
        return new Company
        {
            UserId = chatterId,
            CompanyName = chatterId + "_NAME",
            DisplayName = chatterName,
            IsBroadcaster = (chatterName == BROADCASTER_NAME),
            Points = 0,
            Employees = new List<EmployeeQuantity>
            {
                new()
                {
                    Type = EmployeeType.Sales,
                    Quantity = 1
                },
                new()
                {
                    Type = EmployeeType.Production,
                    Quantity = 1
                }
            }
        };
    }

    #endregion
}