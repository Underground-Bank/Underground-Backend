using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Commands.DeleteAccountNumber;
using UndergroundBank.BankAccountService.Application.Interfaces;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.TopUpAccountNumber
{
    public class TopUpAccountNumberCommandHandler : IRequestHandler<TopUpAccountNumberCommand>
    {
        private readonly IBankService _bankService;

        public TopUpAccountNumberCommandHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task Handle(
            TopUpAccountNumberCommand request,
            CancellationToken cancellationToken
        )
        {
            await _bankService.TopUpAccountNumber(request.accountNumber, request.moneyCount);
        }
    }
}
