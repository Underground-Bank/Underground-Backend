using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Queries.GetAllAccountNumbers;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyAccountNumbers
{
    public class GetMyAccountNumbersQueryHandler
        : IRequestHandler<GetMyAccountNumbersQuery, List<BankAccountDto>>
    {
        private readonly IBankService _bankService;

        public GetMyAccountNumbersQueryHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task<List<BankAccountDto>> Handle(
            GetMyAccountNumbersQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _bankService.GetMyAccountNumbers(request.userId);
        }
    }
}
