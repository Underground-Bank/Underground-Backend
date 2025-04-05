using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Communication.Commands.BlockAccountNumber;
using UndergroundBank.BankAccountService.Application.Interfaces;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.CreateAccountNumber
{
    public class CreateAccountNumberCommandHandler : IRequestHandler<CreateAccountNumberCommand>
    {
        private readonly IBankService _bankService;

        public CreateAccountNumberCommandHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task Handle(
            CreateAccountNumberCommand request,
            CancellationToken cancellationToken
        )
        {
            await _bankService.CreateAccountNumber(request.userId, request.currency);
        }
    }
}
