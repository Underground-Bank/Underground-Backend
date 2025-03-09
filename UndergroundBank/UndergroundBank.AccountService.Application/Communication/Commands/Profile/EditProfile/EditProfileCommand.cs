using MediatR;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Profile.EditProfile
{
    public record EditProfileCommand(EditProfileInfoDto editCreds, string userId) : IRequest { }
}
