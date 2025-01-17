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
        var statistics = await _creditsRepository.GetStatisticsAsync();

        if (statistics != null)
        {
            statistics.TotalPaid = CurrencyHelper.ToStandartUnit(statistics.TotalPaid);
            statistics.TotalAwaitingPayment = CurrencyHelper.ToStandartUnit(statistics.TotalAwaitingPayment);

            var totalAmount = statistics.TotalPaid + statistics.TotalAwaitingPayment;
            
            if (totalAmount != 0)
            {
                statistics.PercentagePaidToTotal = Math.Round(statistics.TotalPaid/totalAmount * 100);
                statistics.PercentageAwaitingPaymentToTotal = Math.Round(
                    statistics.TotalAwaitingPayment/totalAmount * 100);
            }
        }

        return statistics;
    }
}