using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.BlockUser;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.CreateNewEmployee;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.DeleteUser;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.UnblockUser;
using UndergroundBank.AccountService.Application.Communication.Queries.Management.GetAllUsers;
using UndergroundBank.AccountService.Application.Communication.Queries.ProfileService.GetUserProfile;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Helpers;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/management")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Admin)}")]
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 500)]
    public class ManagementController : BaseController
    {
        public ManagementController(IMediator mediator, AdditionalTokenHelper additionalTokenHelper)
            : base(mediator) { }

        [HttpGet("get-users")]
        [ProducesResponseType(typeof(List<ProfileDto>), 200)]
        public async Task<ActionResult<List<ProfileDto>>> GetAllUsers([FromQuery] Role? role)
        {
            var usersQuery = new GetAllUsersQuery(role);
            var usersResponse = await Mediator.Send(usersQuery);

            return Ok(usersResponse);
        }

        [HttpPost("create-employee")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<InputManagerDataDto>> CreateEmployee(ManagerDto managerDto)
        {
            var employeeCommand = new CreateNewEmployeeCommand(managerDto);
            var employee = await Mediator.Send(employeeCommand);

            return Ok(employee);
        }

        [HttpPut("block/{userId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> BlockUser(Guid userId)
        {
            var blockUserCommand = new BlockUserCommand(userId, UserId);
            await Mediator.Send(blockUserCommand);

            return Ok();
        }

        [HttpPut("unblock/{userId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> UnblockUser(Guid userId)
        {
            var unblockUserCommand = new UnblockUserCommand(userId, UserId);
            await Mediator.Send(unblockUserCommand);

            return Ok();
        }

        [HttpPut("delete/{userId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            var deleteUserCommand = new DeleteUserCommand(userId, UserId);
            await Mediator.Send(deleteUserCommand);

            return Ok();
        }
    }
}
