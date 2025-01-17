using System.Data;
using Dapper;

public class InitialCreate
{
    public static async Task CreateAsync(IDbConnection dbConnection)
    {
        dbConnection.Open();
        
        var dropInvoicesTableQuery = "DROP TABLE IF EXISTS Invoices;";
        await dbConnection.ExecuteAsync(dropInvoicesTableQuery);

        var dropCreditsTableQuery = "DROP TABLE IF EXISTS Credits";
        await dbConnection.ExecuteAsync(dropCreditsTableQuery);

        var createCreditsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Credits (
                CreditId INTEGER PRIMARY KEY AUTOINCREMENT,
                CreditNumber TEXT NOT NULL,
                ClientName TEXT NOT NULL,
                RequestedAmount REAL NOT NULL DEFAULT 0,
                CreditRequestDate TEXT NOT NULL DEFAULT (datetime('now')),
                CreditStatus INT NOT NULL DEFAULT 3 CHECK (CreditStatus IN (1, 2, 3))
            )";
        await dbConnection.ExecuteAsync(createCreditsTableQuery);

        var createInvoicesTableQuery = @"
            CREATE TABLE IF NOT EXISTS Invoices (
                InvoiceId INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceNumber TEXT NOT NULL,
                InvoiceAmount REAL NOT NULL DEFAULT 0,
                CreditId INTEGER NOT NULL,
                FOREIGN KEY (CreditId) REFERENCES Credits(CreditId) ON DELETE CASCADE
            )";
        await dbConnection.ExecuteAsync(createInvoicesTableQuery);
    }
}