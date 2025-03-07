using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.TopUpAccountNumber
{
    public record TopUpAccountNumberCommand(string accountNumber, decimal moneyCount) : IRequest;
}
