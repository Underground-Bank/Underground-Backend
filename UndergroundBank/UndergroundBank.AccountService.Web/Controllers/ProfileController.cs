using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.AccountService.Application.Communication.Commands.Profile.ChangePassword;
using UndergroundBank.AccountService.Application.Communication.Commands.Profile.EditProfile;
using UndergroundBank.AccountService.Application.Communication.Queries.ProfileService.GetUserProfile;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Helpers;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize(Policy = "TokenNotInBlackList")]
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 500)]
    public class ProfileController : BaseController
    {
        private readonly AdditionalTokenHelper _additionalTokenHelper;

        public ProfileController(IMediator mediator, AdditionalTokenHelper additionalTokenHelper)
            : base(mediator)
        {
            _additionalTokenHelper = additionalTokenHelper;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ProfileDto), 200)]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var profileQuery = new GetUserProfileQuery(UserId.ToString());
            var profileResponse = await Mediator.Send(profileQuery);

            return Ok(profileResponse);
        }

        [HttpPut()]
        [ProducesResponseType(typeof(Error), 200)]
        public async Task<ActionResult> EditProfile(EditProfileInfoDto editProfileInfo)
        {
            var editCommand = new EditProfileCommand(editProfileInfo, UserId.ToString());
            await Mediator.Send(editCommand);

            return Ok();
        }

        [HttpPut()]
        [Route("change-password")]
        [ProducesResponseType(typeof(Error), 200)]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordCreds)
        {
            var changePasswordCommand = new ChangePasswordCommand(
                UserId.ToString(),
                changePasswordCreds
            );
            await Mediator.Send(changePasswordCommand);

            return Ok();
        }
    }
}
