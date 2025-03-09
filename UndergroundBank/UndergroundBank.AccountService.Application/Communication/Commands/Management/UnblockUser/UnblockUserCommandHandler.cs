using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.BlockUser;
using UndergroundBank.AccountService.Application.Interfaces;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Management.UnblockUser
{
    public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand>
    {
        private readonly IManagementService _managementService;

        public UnblockUserCommandHandler(IManagementService managementService)
        {
            _managementService = managementService;
        }

        public async Task Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            await _managementService.UnblockUser(request.userId, request.currentUserId);
        }
    }
}
