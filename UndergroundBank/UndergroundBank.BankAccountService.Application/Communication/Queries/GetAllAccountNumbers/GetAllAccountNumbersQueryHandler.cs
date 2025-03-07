using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetAllAccountNumbers
{
    public class GetAllAccountNumbersQueryHandler
        : IRequestHandler<GetAllAccountNumbersQuery, List<BankAccountDto>>
    {
        private readonly IBankService _bankService;

        public GetAllAccountNumbersQueryHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task<List<BankAccountDto>> Handle(
            GetAllAccountNumbersQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _bankService.GetAllAccountNumbers();
        }
    }
}
