using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Interfaces;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Profile.EditProfile
{
    public class EditProfileCommandHandler : IRequestHandler<EditProfileCommand>
    {
        private readonly IProfileService _profileService;

        public EditProfileCommandHandler(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task Handle(EditProfileCommand request, CancellationToken cancellationToken)
        {
            await _profileService.EditProfile(request.editCreds, request.userId);
        }
    }
}
