using System.ComponentModel.DataAnnotations.Schema;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.Common.Dto.Transaction;

/// <summary>
/// This DTO is needed for exchange between the history service and other services
/// </summary>
///
///TODO: Refactor it to be more uniform and work with account transfers
public class OperationHistoryDto
{
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
    public TransferEndpoint Destination =>
        new()
        {
            Type = DestinationType,
            LoanId = DestinationLoanId,
            AccountNumber = DestinationAccountNumber,
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

public enum TransactionType
{
    GettingLoanMoney,
    LoanPayment,
    Withdraw,
    TopUp,
}
