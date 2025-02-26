using MediatR;
using UndergroundBank.AccountService.Application.Dto;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Profile.ChangePassword
{
    public record ChangePasswordCommand(string userId, ChangePasswordDto changePasswordCreds)
        : IRequest;
}
