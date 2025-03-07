using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.CreateAccountNumber
{
    public record class CreateAccountNumberCommand(Guid userId) : IRequest;
}
