using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.WithdrawAccountNumber
{
    public record WithdrawAccountNumberCommand(string accountNumber, int moneyCount) : IRequest;
}
