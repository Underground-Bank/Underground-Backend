using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Data.Models;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/user/settings")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 500)]
    public class UserSettingsController : BaseController
    {
        private readonly IUserSettingsService _service;

        public UserSettingsController(IUserSettingsService service, IMediator mediator)
            : base(mediator)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserSettingsDto), 200)]
        public async Task<ActionResult<UserSettingsDto>> GetUserSettings()
        {
            return Ok(await _service.GetUserSettings(UserId));
        }

        [HttpPut]
        [ProducesResponseType(typeof(Error), 200)]
        public async Task<ActionResult> EditUserSettings([FromQuery] EditUserSettingsDto settings)
        {
            await _service.EditUserSettings(UserId, settings);
            return Ok();
        }
    }
}
