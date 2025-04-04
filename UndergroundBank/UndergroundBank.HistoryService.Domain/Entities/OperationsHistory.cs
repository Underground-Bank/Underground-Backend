using System.ComponentModel.DataAnnotations.Schema;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.Middlewares;

public class OperationsHistoryElement
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public Guid UserId { get; set; }

    public string AccountNumber { get; set; } = default!;
    public decimal MoneyCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status { get; set; }
    public TransactionType TransactionType { get; set; }

    public AccountType DestinationType { get; set; }
    public Guid? DestinationLoanId { get; set; }
    public string? DestinationAccountNumber { get; set; }

    [NotMapped]
    public DestinationInfo Destination => new()
    {
        Type = DestinationType,
        LoanId = DestinationLoanId,
        AccountNumber = DestinationAccountNumber
    };

    public void SetDestination(AccountType type, Guid loanId)
    {
        if (type != AccountType.Loan)
            throw new ArgumentException("Метод предназначен только для типа Loan.");

        DestinationType = type;
        DestinationLoanId = loanId;
        DestinationAccountNumber = null;
    }

    public void SetDestination(AccountType type, string accountNumber)
    {
        if (type != AccountType.Account)
            throw new ArgumentException("Метод предназначен только для типа Account.");

        DestinationType = type;
        DestinationAccountNumber = accountNumber;
        DestinationLoanId = null;
    }
}

public class DestinationInfo
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
