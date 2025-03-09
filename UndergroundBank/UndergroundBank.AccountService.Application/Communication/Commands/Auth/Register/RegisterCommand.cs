using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Dto;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Auth.Register
{
    public record RegisterCommand(RegisterInfoDto registerCreds) : IRequest<AuthResponseDto> { }
}
