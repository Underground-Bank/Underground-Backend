using MediatR;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Login;
using UndergroundBank.AccountService.Application.Communication.Commands.Auth.Register;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.Common.Base;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        public AuthController(IMediator mediator)
            : base(mediator) { }

        /// <summary>
        /// TODO: Сделать логин для двух разных сайтов с помощью X-Client-Type или чего то другого
        /// </summary>
        [HttpPost("login")]
        //[ProducesResponseType(typeof(TokenResponseDTO), 200)]
        //[ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        //[ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginInfoDto loginCreds)
        {
            var loginCommand = new LoginCommand(loginCreds);
            var tokenResponse = await Mediator.Send(loginCommand);

            return Ok(tokenResponse);
        }

        [HttpPost("register")]
        //[ProducesResponseType(typeof(TokenResponseDTO), 200)]
        //[ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        //[ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterInfoDto registerCreds)
        {
            var registerCommand = new RegisterCommand(registerCreds);
            var tokenResponse = await Mediator.Send(registerCommand);

            return Ok(tokenResponse);
        }
    }
}
