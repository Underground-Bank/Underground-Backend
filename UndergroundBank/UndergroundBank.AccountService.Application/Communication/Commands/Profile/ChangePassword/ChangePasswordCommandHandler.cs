using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Interfaces;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Profile.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IProfileService _profileService;

        public ChangePasswordCommandHandler(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await _profileService.ChangePassword(request.changePasswordCreds, request.userId);
        }
    }
}
