namespace UndergroundBank.Common.Dto.Transaction
{
    public class CheckBankAccountAccessRequest
    {
        public Guid UserId { get; set; }
        public string BankAccountNumber { get; set; }
    }

    public class CheckBankAccountAccessResponse
    {
        public bool IsBankAccountExists { get; set; }
        public bool HasUserAccess { get; set; }
    }
}
