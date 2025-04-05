using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Logout;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Register;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Helpers;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly AdditionalTokenHelper _additionalTokenHelper;

        public AuthController(IMediator mediator, AdditionalTokenHelper additionalTokenHelper)
            : base(mediator)
        {
            _additionalTokenHelper = additionalTokenHelper;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterInfoDto registerCreds)
        {
            var registerCommand = new RegisterCommand(registerCreds);
            await Mediator.Send(registerCommand);

            return Ok();
        }

        [HttpPost]
        [Authorize(
            AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
        )]
        [Route("logout")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> Logout()
        {
            string token = _additionalTokenHelper.GetTokenFromHeader();

            var logoutCommand = new LogoutCommand(token, UserId.ToString());
            await Mediator.Send(logoutCommand);

            return Ok();
        }
    }
}
