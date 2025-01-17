public class Invoice
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber {get; set; }
    public decimal InvoiceAmount { get; set; }
    public int CreditId { get; set; }
}