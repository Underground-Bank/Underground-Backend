using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyCorrespondingAccountNumber
{
    public record GetMyCorrespondingAccountNumberQuery(string accountNumber)
        : IRequest<BankAccountDto>;
}
