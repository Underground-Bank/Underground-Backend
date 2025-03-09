using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.UnblockUser;
using UndergroundBank.AccountService.Application.Interfaces;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Management.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IManagementService _managementService;

        public DeleteUserCommandHandler(IManagementService managementService)
        {
            _managementService = managementService;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _managementService.DeleteUser(request.userId, request.currentUserId);
        }
    }
}
