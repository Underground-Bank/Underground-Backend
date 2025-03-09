using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Management.UnblockUser
{
    public record UnblockUserCommand(Guid userId, Guid currentUserId) : IRequest;
}
