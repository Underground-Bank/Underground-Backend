using MediatR;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Profile.ChangePassword
{
    public record ChangePasswordCommand(string userId, ChangePasswordDto changePasswordCreds)
        : IRequest;
}
