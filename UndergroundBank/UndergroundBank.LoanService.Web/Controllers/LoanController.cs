using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.Common.Base;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod;
using UndergroundBank.LoanService.Application.Dto.Loan;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/loan")]
    public class LoanController : BaseController
    {
        public LoanController(IMediator mediator)
            : base(mediator) { }

        [HttpPost("take")]
        [Authorize]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> TakeLoan(TakeLoanDto takeLoanDto)
        {
            var takeLoanCommand = new TakeLoanCommand(takeLoanDto, UserId);
            await Mediator.Send(takeLoanCommand);

            return Ok();
        }
    }
}
