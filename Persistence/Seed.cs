using System.Data;
using Dapper;

public class Seed
{
    public static async Task SeedDataAsync(IDbConnection dbConnection)
    {
        await SeedCreditsAsync(dbConnection);
        await SeedInvoicesAsync(dbConnection);
    }

    private static async Task SeedCreditsAsync(IDbConnection dbConnection)
    {
        var credits = new[]
        {
            new 
            { 
                CreditNumber = "GR123244D", 
                ClientName = "Iron Man", 
                RequestedAmount = 10050000, 
                CreditRequestDate = DateTime.UtcNow.AddMonths(-3),
                CreditStatus = 1
            },
            new 
            { 
                CreditNumber = "CA485738L", 
                ClientName = "Capitan America", 
                RequestedAmount = 5000000, 
                CreditRequestDate = DateTime.UtcNow.AddMonths(-6),
                CreditStatus = 2 },
            new { 
                CreditNumber = "SM758120G", 
                ClientName = "Spider-Man", 
                RequestedAmount = 1500000, 
                CreditRequestDate = DateTime.UtcNow.AddMonths(-18),
                CreditStatus = 2 },
            new { 
                CreditNumber = "BP548205B", 
                ClientName = "Black Panther", 
                RequestedAmount = 4000000, 
                CreditRequestDate = DateTime.UtcNow,
                CreditStatus = 3 },
            new { 
                CreditNumber = "HN453023N", 
                ClientName = "Hulk", 
                RequestedAmount = 2500000, 
                CreditRequestDate = DateTime.UtcNow.AddMonths(-5),
                CreditStatus = 2 },
            new {
                CreditNumber = "TR489570M",
                ClientName = "Thor",
                RequestedAmount = 1000000,
                CreditRequestDate = DateTime.UtcNow.AddMonths(-24),
                CreditStatus = 1
            }
        };

        foreach (var credit in credits)
        {
            var existQuery = @"
                SELECT COUNT(1) 
                FROM Credits 
                WHERE CreditNumber = @CreditNumber 
                      AND ClientName = @ClientName;";
            var exist = await dbConnection.ExecuteScalarAsync<bool>(existQuery, 
                new { credit.CreditNumber, credit.ClientName });

            if (!exist)
            {
                var insertQuery = @"INSERT INTO Credits (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatus) 
                                    VALUES (@CreditNumber, @ClientName, @RequestedAmount, @CreditRequestDate, @CreditStatus)";
                await dbConnection.ExecuteAsync(insertQuery, credit);
            }
        }
    }

    public static async Task SeedInvoicesAsync(IDbConnection dbConnection) {
        var invoices = new[]
        {
            new { InvoiceNumber = "3456600", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456601", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456602", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456603", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456604", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456605", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456606", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456607", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456608", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456609", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456610", InvoiceAmount = 62500, CreditId = 3}, 
            new { InvoiceNumber = "3456611", InvoiceAmount = 62500, CreditId = 3}, 
            new { InvoiceNumber = "3456612", InvoiceAmount = 500000, CreditId = 2},
            new { InvoiceNumber = "3456613", InvoiceAmount = 62500, CreditId = 3}, 
            new { InvoiceNumber = "3456614", InvoiceAmount = 250000, CreditId = 5},
            new { InvoiceNumber = "3456615", InvoiceAmount = 500000, CreditId = 2},
            new { InvoiceNumber = "3456616", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456617", InvoiceAmount = 250000, CreditId = 5},
            new { InvoiceNumber = "3456618", InvoiceAmount = 500000, CreditId = 2},
            new { InvoiceNumber = "3456619", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456620", InvoiceAmount = 250000, CreditId = 5},
            new { InvoiceNumber = "3456721", InvoiceAmount = 500000, CreditId = 2},
            new { InvoiceNumber = "3456622", InvoiceAmount = 5025000, CreditId = 1},
            new { InvoiceNumber = "3456623", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456624", InvoiceAmount = 250000, CreditId = 5},
            new { InvoiceNumber = "3456625", InvoiceAmount = 500000, CreditId = 2},
            new { InvoiceNumber = "3456626", InvoiceAmount = 5025000, CreditId = 1},
            new { InvoiceNumber = "3456627", InvoiceAmount = 62500, CreditId = 3},
            new { InvoiceNumber = "3456628", InvoiceAmount = 250000, CreditId = 5},
            new { InvoiceNumber = "3456629", InvoiceAmount = 500000, CreditId = 2},
            new { InvoiceNumber = "3456630", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456731", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456632", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456633", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456634", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456635", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456636", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456637", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456638", InvoiceAmount = 100000, CreditId = 6},
            new { InvoiceNumber = "3456639", InvoiceAmount = 100000, CreditId = 6}         
        };

        foreach (var invoice in invoices)
        {
            var existQuery = @"
                SELECT COUNT(1) 
                FROM Invoices 
                WHERE InvoiceNumber = @InvoiceNumber 
                      AND CreditId = @CreditId;";
            var exist = await dbConnection.ExecuteScalarAsync<bool>(existQuery, 
                new { invoice.InvoiceNumber, invoice.CreditId });

            if (!exist)
            {
                var insertQuery = @"INSERT INTO Invoices (InvoiceNumber, InvoiceAmount, CreditId) 
                                    VALUES (@InvoiceNumber, @InvoiceAmount, @CreditId)";
                await dbConnection.ExecuteAsync(insertQuery, invoice);
            }
        }
    } 
}