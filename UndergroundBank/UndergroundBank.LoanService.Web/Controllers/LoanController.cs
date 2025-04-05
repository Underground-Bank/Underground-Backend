using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.AddAutoTopUpLoan;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.DeleteAutoTopUpLoan;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.TopUpLoan;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetAutoTopUpJobs;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetCreditRating;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetLoan;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetLoans;
using UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetMyLoans;
using UndergroundBank.LoanService.Application.Dto.Loan;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UndergroundBank.LoanService.Web.Controllers
{
    [ApiController]
    [Route("api/loan")]
    [Authorize]
    [Authorize(Policy = "TokenNotInBlackList")]
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 500)]
    public class LoanController : BaseController
    {
        public LoanController(IMediator mediator)
            : base(mediator) { }

        [HttpPost("take")]
        public async Task<ActionResult> TakeLoan(TakeLoanDto takeLoanDto)
        {
            var takeLoanCommand = new TakeLoanCommand(takeLoanDto, UserId);
            await Mediator.Send(takeLoanCommand);

            return Ok();
        }

        [HttpPost("top-up")]
        public async Task<ActionResult> TopUpLoan(TopUpLoanDto topUpLoanDto)
        {
            var TopUpLoanCommand = new TopUpLoanCommand(topUpLoanDto, UserId);
            await Mediator.Send(TopUpLoanCommand);

            return Ok();
        }

        [HttpPost("auto-top-up/add")]
        public async Task<ActionResult> AddAutoTopUpLoan(AutoTopUpLoanDto addAutotopUpLoanDto)
        {
            var addAutoTopUpLoanCommand = new AddAutoTopUpLoanCommand(addAutotopUpLoanDto, UserId);
            await Mediator.Send(addAutoTopUpLoanCommand);

            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetLoanDto), 200)]
        public async Task<ActionResult<GetLoanDto>> GetLoan(Guid id)
        {
            var getLoanQuery = new GetLoanQuery(id);
            var loan = await Mediator.Send(getLoanQuery);

            return Ok(loan);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(GetLoansDto), 200)]
        public async Task<ActionResult<GetLoansDto>> GetAllLoans([FromQuery] Guid? userId)
        {
            var getLoansQuery = new GetLoansQuery(userId);
            var loans = await Mediator.Send(getLoansQuery);

            return Ok(loans);
        }

        [HttpGet("rating")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.Employee)}")]
        [ProducesResponseType(typeof(CreditRatingDto), 200)]
        public async Task<ActionResult<CreditRatingDto>> GetCreditRating([FromQuery] Guid userId)
        {
            var creditRatingQuery = new GetCreditRatingQuery(userId);
            var creditRating = await Mediator.Send(creditRatingQuery);

            return Ok(creditRating);
        }

        [HttpGet("rating/my")]
        [ProducesResponseType(typeof(CreditRatingDto), 200)]
        public async Task<ActionResult<CreditRatingDto>> GetCreditRating()
        {
            var creditRatingQuery = new GetCreditRatingQuery(UserId);
            var creditRating = await Mediator.Send(creditRatingQuery);

            return Ok(creditRating);
        }

        [HttpGet("my")]
        [ProducesResponseType(typeof(GetLoansDto), 200)]
        public async Task<ActionResult<GetLoansDto>> GetMyLoans()
        {
            var getMyLoansQuery = new GetMyLoansQuery(UserId);
            var myLoans = await Mediator.Send(getMyLoansQuery);

            return Ok(myLoans);
        }

        [HttpGet("auto-top-up/my")]
        [ProducesResponseType(typeof(GetAutoTopUpLoanJobsListDto), 200)]
        public async Task<ActionResult<GetAutoTopUpLoanJobsListDto>> GetAutoTopUpJobs()
        {
            var getAutoTopUpJobsQuery = new GetAutoTopUpJobsQuery(UserId);
            var myAutoTopUpJobs = await Mediator.Send(getAutoTopUpJobsQuery);

            return Ok(myAutoTopUpJobs);
        }

        [HttpDelete("auto-top-up/delete")]
        public async Task<ActionResult> DeleteAutoTopUp(AutoTopUpLoanDto deleteAutotopUpLoanDto)
        {
            var deleteAutoTopUpJobCommand = new DeleteAutoTopUpLoanCommand(
                deleteAutotopUpLoanDto,
                UserId
            );
            await Mediator.Send(deleteAutoTopUpJobCommand);

            return Ok();
        }
    }
}
