using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.CreateAccountNumber
{
    public record class CreateAccountNumberCommand(Guid userId, Currency currency) : IRequest;
}
