using MediatR;
using UndergroundBank.AccountService.Application.Dto;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Auth.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequestDto refreshTokenRequestCreds)
        : IRequest<AuthResponseDto>;
}
