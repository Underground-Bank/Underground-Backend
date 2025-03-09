using MediatR;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Queries.ProfileService.GetUserProfile
{
    public record GetUserProfileQuery(string userId) : IRequest<ProfileDto>;
}
