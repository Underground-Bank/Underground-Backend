using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyAccountNumbers;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyCorrespondingAccountNumber
{
    public class GetMyCorrespondingAccountNumberQueryHandler
        : IRequestHandler<GetMyCorrespondingAccountNumberQuery, BankAccountDto>
    {
        private readonly IBankService _bankService;

        public GetMyCorrespondingAccountNumberQueryHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task<BankAccountDto> Handle(
            GetMyCorrespondingAccountNumberQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _bankService.GetMyCorrespondingAccountNumber(request.accountNumber);
        }
    }
}
