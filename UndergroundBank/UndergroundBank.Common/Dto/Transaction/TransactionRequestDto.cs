using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.Common.Dto.Transaction;

public class TransactionRequestDto
{
    public required Guid TransactionId { get; set; }
    public required Guid UserId { get; set; }

    public required TransferEndpoint From { get; set; }
    public required TransferEndpoint To { get; set; }
    public required Currency Currency { get; set; }
    public decimal MoneyCount { get; set; }
    public required Status Status { get; set; }
}

public class TransferEndpoint
{
    public required AccountType Type { get; set; }

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
    Loan,
}
