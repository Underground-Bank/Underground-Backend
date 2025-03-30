using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.HistoryService.Application.Communication.Queries.GetAccountNumbersWithUserId;
using UndergroundBank.HistoryService.Application.Communication.Queries.GetOverduePayments;
using UndergroundBank.HistoryService.Application.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UndergroundBank.HistoryService.Web.Controllers
{
    [ApiController]
    [Route("api/history")]
    [Authorize]
    [Authorize(Policy = "TokenNotInBlackList")]
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 500)]
    public class HistoryController : BaseController
    {
        public HistoryController(IMediator mediator)
            : base(mediator) { }

        [HttpGet("{bankAccountNumber}")]
        [ProducesResponseType(typeof(GetOpeationsHistoryDto), 200)]
        public async Task<ActionResult<GetOpeationsHistoryDto>> GetHistoryByBankAccountNumber(string bankAccountNumber)
        {

            Guid? userId = Roles.Contains(Role.Employee) ? UserId : null;

            var getHistoryCommand = new GetHistoryByBankAccountNumberQuery(bankAccountNumber, userId);
            var history = await Mediator.Send(getHistoryCommand);

            return Ok(history);
        }

        [HttpGet("overdue-payments")]
        [ProducesResponseType(typeof(List<OverduePaymentDto>), 200)]
        public async Task<ActionResult<List<OverduePaymentDto>>> GetHistoryByBankAccountNumber([FromQuery] Guid? loanId, [FromQuery] Guid? userId)
        {

            Guid neededUserId = userId == null ? UserId : userId.Value;

            var getOverduePayments = new GetOverduePaymentsQuery(loanId, neededUserId);
            var overduePayments = await Mediator.Send(getOverduePayments);

            return Ok(overduePayments);
        }
    }
}
