using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.BankAccountService.Application.Interfaces;

namespace UndergroundBank.BankAccountService.Application.Communication.Commands.BlockAccountNumber
{
    public class BlockAccountNumberCommandHandler : IRequestHandler<BlockAccountNumberCommand>
    {
        private readonly IBankService _bankService;

        public BlockAccountNumberCommandHandler(IBankService bankService)
        {
            _bankService = bankService;
        }

        public async Task Handle(
            BlockAccountNumberCommand request,
            CancellationToken cancellationToken
        )
        {
            await _bankService.BlockAccountNumber(request.accountNumber);
        }
    }
}
