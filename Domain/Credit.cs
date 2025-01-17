
public class Credit
{
    public int CreditId { get; set; }
    public string CreditNumber { get; set; }
    public string ClientName { get; set; }
    public decimal RequestedAmount { get; set; }
    public DateTime CreditRequestDate { get; set; }

    public Status CreditStatus { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public enum Status
{
    Paid = 1,
    AwaitingPayment = 2,
    Created = 3
}