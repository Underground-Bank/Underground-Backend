using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Dto;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Auth.Login
{
    public record LoginCommand(LoginInfoDto loginCreds) : IRequest<AuthResponseDto>;
}
