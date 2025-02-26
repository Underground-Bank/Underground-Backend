using MediatR;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Queries.ProfileService.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ProfileDto>
    {
        private readonly IProfileService _profileService;

        public GetUserProfileQueryHandler(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<ProfileDto> Handle(
            GetUserProfileQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _profileService.GetUserProfile(request.userId);
        }
    }
}
