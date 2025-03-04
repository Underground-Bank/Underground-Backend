using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.DeleteAccountNumber
{
    public record DeleteAccountNumberCommand(string accountNumber) : IRequest;
}
