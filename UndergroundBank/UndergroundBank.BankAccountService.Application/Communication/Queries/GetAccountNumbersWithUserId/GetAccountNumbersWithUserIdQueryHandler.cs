using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Queries.GetAllAccountNumbers;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetAccountNumbersWithUserId
{
    public class GetAccountNumbersWithUserIdQueryHandler
        : IRequestHandler<GetAccountNumbersWithUserIdQuery, List<BankAccountDto>>
    {
        private readonly IBankService _bankService;

        public GetAccountNumbersWithUserIdQueryHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task<List<BankAccountDto>> Handle(
            GetAccountNumbersWithUserIdQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _bankService.GetAccountNumbersWithUserId(request.userId);
        }
    }
}
