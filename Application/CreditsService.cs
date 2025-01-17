public class CreditsService : ICreditsService
{    
    private readonly ICreditsRepository _creditsRepository;

    public  CreditsService(ICreditsRepository creditsRepository)
    {
        _creditsRepository = creditsRepository;
    }

    public async Task<List<Credit>> GetCreditListAsync()
    {
        return await _creditsRepository.GetListAsync();
    }

    public async Task<CreditStatistics> GetCreditStatisticsAsync()
    {
        var statistic = await _creditsRepository.GetStatisticsAsync();

        if (statistic != null)
        {
            var totalAmount = statistic.TotalPaid + statistic.TotalAwaitingPayment;
            
            if (totalAmount != 0)
            {
                statistic.PercentagePaidToTotal = Math.Round(statistic.TotalPaid/totalAmount * 100);
                statistic.PercentageAwaitingPaymentToTotal = Math.Round(
                    statistic.TotalAwaitingPayment/totalAmount * 100);
            }
        }

        return statistic;
    }
}