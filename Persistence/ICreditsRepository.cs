public interface ICreditsRepository
{
    Task<List<Credit>> GetListAsync();
    Task<CreditStatistics> GetStatisticsAsync();
}