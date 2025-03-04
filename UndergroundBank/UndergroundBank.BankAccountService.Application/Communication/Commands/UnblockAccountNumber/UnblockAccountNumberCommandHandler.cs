using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Commands.TopUpAccountNumber;
using UndergroundBank.BankAccountService.Application.Interfaces;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.UnblockAccountNumber
{
    public class UnblockAccountNumberCommandHandler : IRequestHandler<UnblockAccountNumberCommand>
    {
        private readonly IBankService _bankService;

        public UnblockAccountNumberCommandHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task Handle(
            UnblockAccountNumberCommand request,
            CancellationToken cancellationToken
        )
        {
            await _bankService.UnblockAccountNumber(request.accountNumber);
        }
    }
}
