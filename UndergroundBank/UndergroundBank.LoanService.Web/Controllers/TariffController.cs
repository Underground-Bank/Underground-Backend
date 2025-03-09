using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod;
using UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariff;
using UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariffs;
using UndergroundBank.LoanService.Application.Dto.Tariff;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UndergroundBank.LoanService.Web.Controllers
{
    [ApiController]
    [Route("api/tariff")]
    [Authorize]
    [Authorize(Policy = "TokenNotInBlackList")]
    [ProducesResponseType(typeof(Error), 400)]
    [ProducesResponseType(typeof(Error), 500)]
    public class TariffController : BaseController
    {
        public TariffController(IMediator mediator)
            : base(mediator) { }

        [HttpPost("create")]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Admin)}")]
        public async Task<ActionResult> CreateTariff(CreateTariffDto createTariffDto)
        {
            var createTariffCommand = new CreateTariffCommand(createTariffDto);
            await Mediator.Send(createTariffCommand);

            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetTariffDto), 200)]
        [ProducesResponseType(typeof(Error), 404)]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Admin)}")]
        public async Task<ActionResult<GetTariffDto>> GetTariff(Guid id)
        {
            var getTariffQuery = new GetTariffQuery(id);
            var tariff = await Mediator.Send(getTariffQuery);

            return Ok(tariff);
        }

        [HttpGet("getAll")]
        [ProducesResponseType(typeof(GetTariffsDto), 200)]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Admin)}")]
        public async Task<ActionResult<GetTariffsDto>> GetAllTarifs()
        {
            var getAllTariffsQuery = new GetTariffsQuery();
            var tariffs = await Mediator.Send(getAllTariffsQuery);

            return Ok(tariffs);
        }
    }
}
