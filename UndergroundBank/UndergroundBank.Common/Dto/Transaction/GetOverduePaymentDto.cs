namespace UndergroundBank.Common.DTO.Transaction;

public class GetOverduePaymentDto
{
    public decimal MoneyCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AccountNumber { get; set; }
}
