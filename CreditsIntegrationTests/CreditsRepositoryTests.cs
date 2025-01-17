using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using NUnit.Framework.Internal;

[TestFixture]
public class CreditsRepositoryTests
{
    private IDbConnection _dbConnection;
    private ICreditsRepository _creditsRepository;
    private List<Credit> _initialCredits = new List<Credit>();

    [SetUp]
    public async Task Setup()
    {
        _dbConnection = new SqliteConnection("DataSource=:memory:");
        _dbConnection.Open();

        _creditsRepository = new CreditsRepository(_dbConnection);

        await InitialCreate.CreateAsync(_dbConnection);
        _initialCredits = await SeedDataAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _dbConnection.Close();
        _dbConnection.Dispose();
    }

    [Test]
    public async Task GetListAsync_CreditsWithInvoices_ShouldReturnListOfCredits()
    {
        var credits = await _creditsRepository.GetListAsync();

        Assert.That(credits, Is.Not.Null);
        Assert.That(credits.Count, Is.EqualTo(2));

        var firstCredit = credits[0];
        Assert.That(firstCredit.CreditNumber, Is.EqualTo(_initialCredits[0].CreditNumber));
        Assert.That(firstCredit.ClientName, Is.EqualTo(_initialCredits[0].ClientName));
        Assert.That(firstCredit.RequestedAmount, Is.EqualTo(_initialCredits[0].RequestedAmount));
        Assert.That(firstCredit.CreditRequestDate, Is.EqualTo(_initialCredits[0].CreditRequestDate));
        Assert.That(firstCredit.CreditStatus, Is.EqualTo(_initialCredits[0].CreditStatus));

        Assert.That(firstCredit.Invoices.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetListAsync_CreditWithoutInvoices_ShouldReturnEmptyListOfInvoices()
    {
        var creditWithoutInvoice = await SeedCreditWithoutInvoicesAsync();
        var credits = await _creditsRepository.GetListAsync();

        Assert.That(credits, Is.Not.Null);
        Assert.That(credits.Count, Is.EqualTo(3));

        Assert.That(credits.Any(c => c.Invoices.Count == 0), Is.True);

        var creditResult = credits.First(c => c.Invoices.Count == 0);

        Assert.That(creditResult.CreditId, Is.EqualTo(creditWithoutInvoice.CreditId));
    }

    [Test]
    public async Task GetStatisticsAsync_ShouldReturnCorrectStatistics()
    {
        var statistics = await _creditsRepository.GetStatisticsAsync();

        var totalPaid = 1000;
        var totalAwaitingPayment = 10000;

        Assert.That(statistics, Is.Not.Null);
        Assert.That(statistics.TotalPaid, Is.EqualTo(totalPaid));
        Assert.That(statistics.TotalAwaitingPayment, Is.EqualTo(totalAwaitingPayment));
    }

    private async Task<List<Credit>> SeedDataAsync()
    {
        var credits = new List<Credit>()
        {
            new Credit
            {
                CreditId = 1,
                CreditNumber = "LS123456R",
                ClientName = "Luke Skywalker",
                RequestedAmount = 1000,
                CreditRequestDate = new DateTime(2024, 3, 5),
                CreditStatus = Status.Paid,
                Invoices = new List<Invoice>()
                {
                    new Invoice
                    {
                        InvoiceId = 1,
                        InvoiceNumber = "123456",
                        InvoiceAmount = 500,
                        CreditId = 1
                    },
                    new Invoice
                    {
                        InvoiceId = 2,
                        InvoiceNumber = "123457",
                        InvoiceAmount = 500,
                        CreditId = 1
                    }
                }
            },
            new Credit
            {
                CreditId = 2,
                CreditNumber = "PL234567S",
                ClientName = "Princess Leia",
                RequestedAmount = 10000,
                CreditRequestDate = new DateTime(2024, 11, 17),
                CreditStatus = Status.AwaitingPayment,
                Invoices = new List<Invoice>()
                {
                    new Invoice
                    {
                        InvoiceId = 3,
                        InvoiceNumber = "234567",
                        InvoiceAmount = 1000,
                        CreditId = 2
                    },
                    new Invoice
                    {
                        InvoiceId = 4,
                        InvoiceNumber = "234568",
                        InvoiceAmount = 1000,
                        CreditId = 2
                    },
                    new Invoice
                    {
                        InvoiceId = 4,
                        InvoiceNumber = "234569",
                        InvoiceAmount = 1000,
                        CreditId = 2
                    }
                }
            }
        };

        var insertCreditsSql = @"
            INSERT INTO Credits (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatus)
                VALUES
                ('LS123456R', 'Luke Skywalker', 1000, '2024-03-05', 1),
                ('PL234567S', 'Princess Leia', 10000, '2024-11-17', 2);
            ";
        await _dbConnection.ExecuteAsync(insertCreditsSql);

        var insertInvoicesSql = @"
            INSERT INTO Invoices (InvoiceNumber, InvoiceAmount, CreditId)
                VALUES
                ('123456', 500, 1),
                ('123457', 500, 1),
                ('234567', 1000, 2),
                ('234568', 1000, 2),
                ('234569', 1000, 2);
            ";
        await _dbConnection.ExecuteAsync(insertInvoicesSql);

        return credits;
    }

    private async Task<Credit> SeedCreditWithoutInvoicesAsync()
    {
        var credit = new Credit
        {
            CreditId = 3,
            CreditNumber = "MY123456P",
            ClientName = "Magister Yoda",
            RequestedAmount = 2000,
            CreditRequestDate = DateTime.UtcNow,
            CreditStatus = Status.Created
        };

        var insertCreditSql = $@"
            INSERT INTO Credits (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatus)
                VALUES
                (@CreditNumber, @ClientName, @RequestedAmount, @CreditRequestDate, @CreditStatus);
            ";

        await _dbConnection.ExecuteAsync(insertCreditSql, credit);

        _initialCredits.Add(credit);

        return credit;
    }
}