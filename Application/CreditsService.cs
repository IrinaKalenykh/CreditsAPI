public class CreditsService : ICreditsService
{    
    private readonly ICreditsRepository _creditsRepository;

    public  CreditsService(ICreditsRepository creditsRepository)
    {
        _creditsRepository = creditsRepository;
    }

    public async Task<List<Credit>> GetCreditListAsync()
    {
        var credits = await _creditsRepository.GetListAsync();

        credits.ForEach(credit => 
        {
                credit.RequestedAmount = CurrencyHelper.ToStandartUnit(credit.RequestedAmount);

                foreach(var invoice in credit.Invoices)
                {
                    invoice.InvoiceAmount = CurrencyHelper.ToStandartUnit(invoice.InvoiceAmount);
                }
        });

        return credits;
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