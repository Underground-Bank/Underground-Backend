using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Commands.CreateAccountNumber;
using UndergroundBank.BankAccountService.Application.Interfaces;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.DeleteAccountNumber
{
    public class DeleteAccountNumberCommandHandler : IRequestHandler<DeleteAccountNumberCommand>
    {
        private readonly IBankService _bankService;

        public DeleteAccountNumberCommandHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task Handle(
            DeleteAccountNumberCommand request,
            CancellationToken cancellationToken
        )
        {
            await _bankService.DeleteAccountNumber(request.accountNumber);
        }
    }
}
