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
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 500)]
    public class HistoryController : BaseController
    {
        public HistoryController(IMediator mediator)
            : base(mediator) { }

        [HttpGet("{bankAccountNumber}")]
        [ProducesResponseType(typeof(GetOpeationsHistoryDto), 200)]
        public async Task<ActionResult<GetOpeationsHistoryDto>> GetHistoryByBankAccountNumber(
            string bankAccountNumber
        )
        {
            Guid? userId = Roles.Contains(Role.Employee) ? UserId : null;

            var getHistoryCommand = new GetHistoryByBankAccountNumberQuery(
                bankAccountNumber,
                userId
            );
            var history = await Mediator.Send(getHistoryCommand);

            return Ok(history);
        }

        [HttpGet("overdue-payments/my")]
        [ProducesResponseType(typeof(List<OverduePaymentDto>), 200)]
        public async Task<ActionResult<List<OverduePaymentDto>>> GetMyOverduedPayments([FromQuery] Guid? loanId)
        {
            var getOverduePayments = new GetOverduePaymentsQuery(loanId, UserId);
            var overduePayments = await Mediator.Send(getOverduePayments);

            return Ok(overduePayments);
        }

        [HttpGet("overdue-payments/{userId}")]
        [ProducesResponseType(typeof(List<OverduePaymentDto>), 200)]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.Employee)}")]
        public async Task<ActionResult<List<OverduePaymentDto>>> GetOverduedPaymentsByUserId(Guid userId, [FromQuery] Guid? loanId)
        {
            var getOverduePayments = new GetOverduePaymentsQuery(loanId, userId);
            var overduePayments = await Mediator.Send(getOverduePayments);

            return Ok(overduePayments);
        }

    }
}
