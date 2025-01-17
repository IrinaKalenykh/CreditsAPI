using System.Data;
using Dapper;

public class CreditsRepository : ICreditsRepository
{
    private readonly IDbConnection _dbConnection;

    public CreditsRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<Credit>> GetListAsync()
    {
        var sql = @"
            SELECT c.CreditId, c.CreditNumber, c.ClientName, c.RequestedAmount, c.CreditRequestDate, c.CreditStatus,
                   i.CreditId as Id,
                   i.InvoiceId, i.InvoiceNumber, i.InvoiceAmount, i.CreditId
            FROM Credits c
            LEFT JOIN Invoices i ON c.CreditId = i.CreditId
            ORDER BY c.CreditId
            ";
        
        var creditDictionary = new Dictionary<int, Credit>();

        var result = await _dbConnection.QueryAsync<Credit, Invoice, Credit>(sql, 
            (credit, invoice) =>
            {
                if (!creditDictionary.TryGetValue(credit.CreditId, out var creditEntry))
                {
                    creditEntry = credit;
                    creditDictionary.Add(credit.CreditId, creditEntry);
                }

                if (invoice.InvoiceId != default(int))
                {
                    creditEntry.Invoices.Add(invoice);
                }

                return creditEntry;
            },
            splitOn: "Id"
            );

        return creditDictionary.Values.ToList();
    }

    public async Task<CreditStatistics> GetStatisticsAsync()
    {
        var sql = @"
            SELECT sum(CASE WHEN CreditStatus = 1 THEN RequestedAmount ELSE 0 END) AS TotalPaid,
                   sum(CASE WHEN CreditStatus = 2 THEN RequestedAmount ELSE 0 END) AS TotalAwaitingPayment
            FROM Credits
            WHERE CreditStatus IN (1, 2);
        ";

        return await _dbConnection.QuerySingleAsync<CreditStatistics>(sql);
    }
}