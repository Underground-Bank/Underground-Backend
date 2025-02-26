using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Login;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Logout;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.RefreshToken;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Register;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Infrastructure.Helpers.TokenHerlpers;
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

        /// <summary>
        /// TODO: Сделать логин для двух разных сайтов с помощью X-Client-Type или чего то другого
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginInfoDto loginCreds)
        {
            var loginCommand = new LoginCommand(loginCreds);
            var tokenResponse = await Mediator.Send(loginCommand);

            return Ok(tokenResponse);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterInfoDto registerCreds)
        {
            var registerCommand = new RegisterCommand(registerCreds);
            var tokenResponse = await Mediator.Send(registerCommand);

            return Ok(tokenResponse);
        }

        [HttpPost]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("logout")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> Logout()
        {
            string token = _additionalTokenHelper.GetTokenFromHeader();
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            var logoutCommand = new LogoutCommand(token, userId);
            await Mediator.Send(logoutCommand);

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "TokenNotInBlackList")]
        [Route("refresh-token")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(
            RefreshTokenRequestDto refreshTokenRequestCreds
        )
        {
            var refreshTokenCommand = new RefreshTokenCommand(refreshTokenRequestCreds);
            var newTokens = await Mediator.Send(refreshTokenCommand);

            return Ok(newTokens);
        }
    }
}
