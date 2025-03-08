using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.BankAccountService.Application.Communication.Commands.BlockAccountNumber;
using UndergroundBank.BankAccountService.Application.Communication.Commands.CreateAccountNumber;
using UndergroundBank.BankAccountService.Application.Communication.Commands.DeleteAccountNumber;
using UndergroundBank.BankAccountService.Application.Communication.Commands.TopUpAccountNumber;
using UndergroundBank.BankAccountService.Application.Communication.Commands.UnblockAccountNumber;
using UndergroundBank.BankAccountService.Application.Communication.Commands.WithdrawAccountNumber;
using UndergroundBank.BankAccountService.Application.Communication.Queries.GetAccountNumbersWithUserId;
using UndergroundBank.BankAccountService.Application.Communication.Queries.GetAllAccountNumbers;
using UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyAccountNumbers;
using UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyCorrespondingAccountNumber;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.BankAccountService;
using UndergroundBank.Common.Helpers;

namespace UndergroundBank.BankAccountService.Web.Controllers
{
    [ApiController]
    [Route("api/bank-account")]
    [Authorize(Policy = "TokenNotInBlackList")]
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 401)]
    [ProducesResponseType(typeof(Error), 500)]
    public class BankAccountController : BaseController
    {
        public BankAccountController(IMediator mediator)
            : base(mediator) { }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Admin)}")]
        [Route("block")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> BlockBankAccount([FromQuery] string accountNumber)
        {
            var blockAccountNumberCommand = new BlockAccountNumberCommand(accountNumber);
            await Mediator.Send(blockAccountNumberCommand);

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Admin)}")]
        [Route("unblock")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> UnlockBankAccount([FromQuery] string accountNumber)
        {
            var unblockAccountNumberCommand = new UnblockAccountNumberCommand(accountNumber);
            await Mediator.Send(unblockAccountNumberCommand);

            return Ok();
        }

        [HttpPost("create")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> CreateBankAccount()
        {
            var createBankAccountNumberCommand = new CreateAccountNumberCommand(UserId);
            await Mediator.Send(createBankAccountNumberCommand);

            return Ok();
        }

        [HttpPost("top-up")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> TopUpAccountNumber(
            [FromQuery] string accountNumber,
            decimal moneyCount = 0
        )
        {
            var topUpAccountNumberCommand = new TopUpAccountNumberCommand(
                accountNumber,
                moneyCount
            );
            await Mediator.Send(topUpAccountNumberCommand);

            return Ok();
        }

        [HttpPost("withdraw")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> WithdrawAccountNumber(
            [FromQuery] string accountNumber,
            decimal moneyCount = 0
        )
        {
            var withdrawAccountNumberCommand = new WithdrawAccountNumberCommand(
                accountNumber,
                moneyCount,
                UserId
            );
            await Mediator.Send(withdrawAccountNumberCommand);

            return Ok();
        }

        [HttpDelete("delete")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteAccountNumber([FromQuery] string accountNumber)
        {
            var deleteAccountNumberCommand = new DeleteAccountNumberCommand(accountNumber);
            await Mediator.Send(deleteAccountNumberCommand);

            return Ok();
        }

        [HttpGet("all")]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Admin)}")]
        [ProducesResponseType(typeof(BankAccountDto), 200)]
        public async Task<ActionResult<List<BankAccountDto>>> GetAllAccountNumbers()
        {
            var bankAccountQuery = new GetAllAccountNumbersQuery();
            var bankAccountResponse = await Mediator.Send(bankAccountQuery);

            return Ok(bankAccountResponse);
        }

        [HttpGet("my")]
        [ProducesResponseType(typeof(BankAccountDto), 200)]
        public async Task<ActionResult<List<BankAccountDto>>> GetMyAccountNumbers()
        {
            var bankAccountQuery = new GetMyAccountNumbersQuery(UserId);
            var bankAccountResponse = await Mediator.Send(bankAccountQuery);

            return Ok(bankAccountResponse);
        }

        [HttpGet("corresponding")]
        [ProducesResponseType(typeof(BankAccountDto), 200)]
        public async Task<ActionResult<BankAccountDto>> GetCorrespondingAccountNumber(
            [FromQuery] string accountNumber
        )
        {
            var bankAccountQuery = new GetMyCorrespondingAccountNumberQuery(
                accountNumber,
                UserId,
                Roles
            );

            var bankAccountResponse = await Mediator.Send(bankAccountQuery);

            return Ok(bankAccountResponse);
        }

        [HttpGet("corresponding/{userId}")]
        [ProducesResponseType(typeof(BankAccountDto), 200)]
        public async Task<ActionResult<BankAccountDto>> GetCorrespondingAccountNumbersWithUserId(
            Guid userId
        )
        {
            var bankAccountQuery = new GetAccountNumbersWithUserIdQuery(userId);
            var bankAccountResponse = await Mediator.Send(bankAccountQuery);

            return Ok(bankAccountResponse);
        }
    }
}
