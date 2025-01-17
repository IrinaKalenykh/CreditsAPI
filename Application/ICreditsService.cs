public interface ICreditsService
{
    Task<List<Credit>> GetCreditListAsync();
    Task<CreditStatistics> GetCreditStatisticsAsync();
}