using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Interfaces;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponseDto> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken
        )
        {
            return await _authService.RefreshToken(request.refreshTokenRequestCreds);
        }
    }
}
