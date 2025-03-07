using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.BlockAccountNumber
{
    public record BlockAccountNumberCommand(string accountNumber) : IRequest;
}
