using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Auth.Logout
{
    public record LogoutCommand(string token, string userId) : IRequest { }
}
