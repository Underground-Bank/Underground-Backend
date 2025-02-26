using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.AccountService.Application.Communication.Queries.ProfileService.GetUserProfile;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Helpers;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : BaseController
    {
        private readonly AdditionalTokenHelper _additionalTokenHelper;

        public ProfileController(IMediator mediator, AdditionalTokenHelper additionalTokenHelper)
            : base(mediator)
        {
            _additionalTokenHelper = additionalTokenHelper;
        }

        [HttpGet()]
        [Authorize(Policy = "TokenNotInBlackList")]
        [ProducesResponseType(typeof(ProfileDto), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            var profileQuery = new GetUserProfileQuery(userId);
            var profileResponse = await Mediator.Send(profileQuery);

            return Ok(profileResponse);
        }
    }
}
