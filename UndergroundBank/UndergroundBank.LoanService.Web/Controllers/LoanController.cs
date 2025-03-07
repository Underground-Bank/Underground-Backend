using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.Common.Base;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.AddAutoTopUpLoan;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.TopUpLoan;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetLoan;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetLoans;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetMyLoans;
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

        [HttpPost("top-up")]
        [Authorize]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> TopUpLoan(TopUpLoanDto topUpLoanDto)
        {
            var TopUpLoanCommand = new TopUpLoanCommand(topUpLoanDto);
            await Mediator.Send(TopUpLoanCommand);

            return Ok();
        }

        [HttpPost("auto-top-up/add")]
        [Authorize]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> AddAutoTopUpLoan(AddAutoTopUpLoanDto addAutotopUpLoanDto)
        {
            var addAutoTopUpLoanCommand = new AddAutoTopUpLoanCommand(addAutotopUpLoanDto);
            await Mediator.Send(addAutoTopUpLoanCommand);

            return Ok();
        }

        [HttpPost("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetLoanDto>> GetLoan(Guid id)
        {
            var getLoanQuery = new GetLoanQuery(id);
            var loan = await Mediator.Send(getLoanQuery);

            return Ok(loan);
        }

        [HttpGet("all")]
        [Authorize]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetLoansDto>> GetAllLoans()
        {
            var getLoansQuery = new GetLoansQuery();
            var loans = await Mediator.Send(getLoansQuery);

            return Ok(loans);
        }

        [HttpGet("my")]
        [Authorize]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetLoansDto>> GetMyLoans()
        {
            var getMyLoansQuery = new GetMyLoansQuery();
            var myLoans = await Mediator.Send(getMyLoansQuery);

            return Ok(myLoans);
        }
    }
}
