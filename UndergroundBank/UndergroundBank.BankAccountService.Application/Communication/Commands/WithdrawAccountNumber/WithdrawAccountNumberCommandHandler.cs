using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Commands.TopUpAccountNumber;
using UndergroundBank.BankAccountService.Application.Interfaces;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.WithdrawAccountNumber
{
    public class WithdrawAccountNumberCommandHandler : IRequestHandler<WithdrawAccountNumberCommand>
    {
        private readonly IBankService _bankService;

        public WithdrawAccountNumberCommandHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task Handle(
            WithdrawAccountNumberCommand request,
            CancellationToken cancellationToken
        )
        {
            await _bankService.WithdrawAccountNumber(request.accountNumber, request.moneyCount);
        }
    }
}
