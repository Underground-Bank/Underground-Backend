using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Logout;
using UndergroundBank.AccountService.Application.Interfaces;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Management.BlockUser
{
    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand>
    {
        private readonly IManagementService _managementService;

        public BlockUserCommandHandler(IManagementService managementService)
        {
            _managementService = managementService;
        }

        public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            await _managementService.BlockUser(request.userId, request.currentUserId);
        }
    }
}
