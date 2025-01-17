using Bogus;
using Moq;

[TestFixture]
public class CreditsServiceTests
{    
    private Mock<ICreditsRepository> _creditsRepositoryMock;
    private ICreditsService _creditsService;
    
    [SetUp]
    public void Setup()
    {
        _creditsRepositoryMock = new Mock<ICreditsRepository>();
        _creditsService = new CreditsService(_creditsRepositoryMock.Object);
    }

    [Test]
    public async Task GetCreditListAsync_WithCredits_ShouldReturnListOfCredits()
    {
        var testCredits = CreateCreditsForTest();

        _creditsRepositoryMock.Setup(repository => repository.GetListAsync())
            .ReturnsAsync(testCredits);

        var result = await _creditsService.GetCreditListAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result, Is.EqualTo(testCredits));
    }

    [Test]
    public async Task GetCreditListAsync_NoCredits_ShouldReturnEmptyListOfCredits()
    {
        var testCredits = new List<Credit>();

        _creditsRepositoryMock.Setup(repository => repository.GetListAsync())
            .ReturnsAsync(testCredits);

        var result = await _creditsService.GetCreditListAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetCreditStatisticAsync_TotalAmoutIsZero_ShouldReturnZeroPercentage()
    {
        var totalPaid = 0m;
        var totalAwaitingPayment = 0m;
        var testPercentagePaid = 0m;
        var testPercentageAwaitingPayment = 0m;

        var testStatistic = new CreditStatistics 
        {
            TotalPaid = totalPaid,
            TotalAwaitingPayment = totalAwaitingPayment
        };

        _creditsRepositoryMock.Setup(repository => repository.GetStatisticsAsync())
            .ReturnsAsync(testStatistic);

        var result = await _creditsService.GetCreditStatisticsAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalPaid, Is.EqualTo(totalPaid));
        Assert.That(result.TotalAwaitingPayment, Is.EqualTo(totalAwaitingPayment));
        Assert.That(result.PercentagePaidToTotal, Is.EqualTo(testPercentagePaid));
        Assert.That(result.PercentageAwaitingPaymentToTotal, 
            Is.EqualTo(testPercentageAwaitingPayment));
    }

    [Test]
    public async Task GetCreditStatisticAsync_TotalAmoutIsNotZero_ShouldReturnCorrectPercentage()
    {
        var totalPaid = 3000m;
        var totalAwaitingPayment = 2000m;
        var testPercentagePaid = 60m;
        var testPercentageAwaitingPayment = 40m;

        var testStatistic = new CreditStatistics 
        {
            TotalPaid = totalPaid,
            TotalAwaitingPayment = totalAwaitingPayment
        };

        _creditsRepositoryMock.Setup(repository => repository.GetStatisticsAsync())
            .ReturnsAsync(testStatistic);

        var result = await _creditsService.GetCreditStatisticsAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalPaid, Is.EqualTo(totalPaid));
        Assert.That(result.TotalAwaitingPayment, Is.EqualTo(totalAwaitingPayment));
        Assert.That(result.PercentagePaidToTotal, Is.EqualTo(testPercentagePaid));
        Assert.That(result.PercentageAwaitingPaymentToTotal, Is.EqualTo(testPercentageAwaitingPayment));
    }

    private List<Credit> CreateCreditsForTest()
    {
        var statuses = new[] { 1, 2, 3};
        var invoiceFaker = new Faker<Invoice>()
        .RuleFor(i => i.InvoiceId, f => f.IndexFaker + 1)
        .RuleFor(i => i.InvoiceNumber, f => f.Random.Replace("########"))
        .RuleFor(i => i.InvoiceAmount, f => f.Finance.Amount());

        var creditFaker = new Faker<Credit>()
            .RuleFor(c => c.CreditId, f => f.IndexFaker + 1)
            .RuleFor(c => c.CreditNumber, f => f.Random.Replace("DK######J"))
            .RuleFor(c => c.ClientName, f => f.Name.FullName())
            .RuleFor(c => c.RequestedAmount, f => f.Finance.Amount(50, 20000))
            .RuleFor(c => c.CreditRequestDate, f => f.Date.Past(2))
            .RuleFor(c => c.CreditStatus, f => f.PickRandom<Status>())
            .RuleFor(c => c.Invoices, f => invoiceFaker.Generate(f.Random.Int(1, 5)));

        var credits = creditFaker.Generate(5);

        credits.ForEach(credit => 
        {
            foreach (var invoice in credit.Invoices)
            {
                invoice.CreditId = credit.CreditId;
            }
        });

        return credits;
    }
}