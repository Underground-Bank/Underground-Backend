using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.Common.Dto.Transaction;

public class TransactionRequestDto
{
    public Guid TransactionId { get; set; }
    public Guid UserId { get; set; }

    public TransferEndpoint From { get; set; }
    public TransferEndpoint To { get; set; }

    public decimal MoneyCount { get; set; }
    public Status Status { get; set; }
}

public class TransferEndpoint
{
    public AccountType Type { get; set; }

    public Guid? LoanId { get; set; }
    public string? AccountNumber { get; set; }

    public Guid GetLoanId()
    {
        if (Type != AccountType.Loan)
            throw new BadRequestException("Тип должен быть 'Loan' для получения LoanId");

        return LoanId ?? throw new BadRequestException("LoanId не задан");
    }

    public string GetAccountNumber()
    {
        if (Type != AccountType.Account)
            throw new BadRequestException("Тип должен быть 'Account' для получения номера счета");

        return AccountNumber ?? throw new BadRequestException("AccountNumber не задан");
    }
}

public enum AccountType
{
    Account,
    Loan
}

