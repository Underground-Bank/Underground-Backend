using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Interfaces;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponseDto> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken
        )
        {
            return await _authService.Register(request.registerCreds);
        }
    }
}
