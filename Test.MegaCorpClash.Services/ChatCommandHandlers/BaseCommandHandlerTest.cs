using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public abstract class BaseCommandHandlerTest
{
    private const char COMMAND_PREFIX = '!';

    protected const string POINTS_NAME = "CorpoBux";
    protected const string DEFAULT_CHATTER_ID = "123";
    protected const string DEFAULT_CHATTER_DISPLAY_NAME = "CodingWithScott";

    internal static GameSettings GetDefaultGameSettings()
    {
        var gameSettings = new GameSettings
        {
            TwitchAccounts = new List<GameSettings.TwitchAccount>()
            {
                new()
                {
                    Type = "Broadcaster",
                    Name = "BroadcasterAccount",
                    AuthToken = "123"
                },
                new()
                {
                    Type = "Bot",
                    Name = "BotAccount",
                    AuthToken = "123"
                }
            },
            AttackDetails = new List<GameSettings.AttackDetail>
            {
                new GameSettings.AttackDetail
                {
                    AttackType = "steal",
                    Min = 100,
                    Max = 500
                },
                new GameSettings.AttackDetail
                {
                    AttackType = "strike",
                    Min = 2,
                    Max = 5
                },
                new GameSettings.AttackDetail
                {
                    AttackType = "recruit",
                    Min = 1,
                    Max = 2
                }
            },
            MaxCompanyNameLength = 20,
            MaxMottoLength = 75,
            LowestHrDiscount = 25,
            MinimumSecondsBetweenCommands = 0,
            PointsName = POINTS_NAME,
            TurnDetails = new GameSettings.TurnInfo
            {
                MinutesPerTurn = 2,
                PointsPerTurn = new GameSettings.PointsInfo
                {
                    Lurker = 1,
                    Chatter = 5,
                    SubscriberMultiplier = 2
                }
            },
            StartupDetails = new GameSettings.StartupInfo
            {
                InitialPoints = 50,
                InitialStaff = new List<GameSettings.StartupInfo.StaffDetails>
                {
                    new()
                    {
                        Type = EmployeeType.Production,
                        Quantity = 1
                    },
                    new()
                    {
                        Type = EmployeeType.Sales,
                        Quantity = 1
                    }
                }
            },
            EmployeeHiringDetails = new List<GameSettings.EmployeeHiringInfo>
            {
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.Production,
                    CostToHire = 25
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.Sales,
                    CostToHire = 50
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.Marketing,
                    CostToHire = 75
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.Research,
                    CostToHire = 250
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.HR,
                    CostToHire = 40
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.Security,
                    CostToHire = 100
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.IT,
                    CostToHire = 100
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.Legal,
                    CostToHire = 150
                },
                new GameSettings.EmployeeHiringInfo
                {
                    Type = EmployeeType.Spy,
                    CostToHire = 500
                }
            }
        };

        return gameSettings;
    }

    internal static GameCommandArgs GetGameCommandArgs(string command)
    {
        var commandWords =
            command.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string commandName = string.Empty;

        if (commandWords[0].StartsWith(COMMAND_PREFIX))
        {
            commandName = commandWords[0][1..];
        }

        return new GameCommandArgs(
            DEFAULT_CHATTER_ID,
            DEFAULT_CHATTER_DISPLAY_NAME,
            commandName,
            string.Join(' ', commandWords.Skip(1)),
            false, false, false, false);
    }

    protected static IRepository GetTestInMemoryRepository(bool includeBroadcasterCompany = true,
        int numberOfNonBroadcasterCompanies = 3)
    {
        var broadcaster =
            new Company
            {
                UserId = "1",
                DisplayName = "CodingWithScott",
                IsBroadcaster = true,
                IsSubscriber = true,
                IsVip = false,
                CreatedOn = DateTime.Now,
                CompanyName = "ScottCo",
                Motto = "Destroy most humans!",
                Points = 1000000
            };

        var player1 = GetPlayerCompany("101", "Player1", true);
        var player2 = GetPlayerCompany("102", "Player2");
        var player3 = GetPlayerCompany("103", "Player3");

        var repository = new InMemoryRepository();

        if(includeBroadcasterCompany)
        {
            repository.AddCompany(broadcaster);
        }

        if (numberOfNonBroadcasterCompanies > 0)
        {
            repository.AddCompany(player1);
        }
        if (numberOfNonBroadcasterCompanies > 1)
        {
            repository.AddCompany(player2);
        }
        if (numberOfNonBroadcasterCompanies > 2)
        {
            repository.AddCompany(player3);
        }

        return repository;
    }

    protected static GameCommandArgs GetGameCommandArgs(Company company, 
        string commandName, string commandArgs)
    {
        return new GameCommandArgs(
            company.UserId,
            company.DisplayName,
            commandName,
            commandArgs,
            company.IsBroadcaster,
            company.IsSubscriber,
            company.IsVip,
            false);
    }

    private static void AddDefaultEmployees(Company company)
    {
        var standardEmployeeList = new List<EmployeeQuantity>
        {
            new EmployeeQuantity
            {
                Type = EmployeeType.Production,
                Quantity = 10
            },
            new EmployeeQuantity
            {
                Type = EmployeeType.Sales,
                Quantity = 10
            }
        };

        foreach (var empQty in standardEmployeeList)
        {
            company.Employees.Add(empQty);
        }
    }

    private static Company GetPlayerCompany(string userId, string playerName,
        bool isSubscriber = false)
    {
        var player =
            new Company
            {
                UserId = userId,
                DisplayName = playerName,
                IsBroadcaster = false,
                IsSubscriber = isSubscriber,
                IsVip = false,
                CreatedOn = DateTime.Now,
                CompanyName = $"{playerName}Co",
                Motto = "We don't need a motto",
                Points = 1000
            };

        AddDefaultEmployees(player);

        return player;
    }
}