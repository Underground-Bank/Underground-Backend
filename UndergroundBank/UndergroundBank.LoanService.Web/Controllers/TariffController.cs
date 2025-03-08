using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.Common.Base;
using UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod;
using UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariff;
using UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariffs;
using UndergroundBank.LoanService.Application.Dto.Tariff;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UndergroundBank.AccountService.Web.Controllers
{
    [ApiController]
    [Route("api/tariff")]
    public class TariffController : BaseController
    {
        public TariffController(IMediator mediator)
            : base(mediator) { }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> CreateTariff(CreateTariffDto createTariffDto)
        {
            var createTariffCommand = new CreateTariffCommand(createTariffDto);
            await Mediator.Send(createTariffCommand);

            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(GetTariffDto), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetTariffDto>> GetTariff(Guid id)
        {
            var getTariffQuery = new GetTariffQuery(id);
            var tariff = await Mediator.Send(getTariffQuery);

            return Ok(tariff);
        }

        [HttpGet("getAll")]
        [Authorize]
        [ProducesResponseType(typeof(GetTariffsDto), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult<GetTariffsDto>> GetAllTarifs()
        {
            var getAllTariffsQuery = new GetTariffsQuery();
            var tariffs = await Mediator.Send(getAllTariffsQuery);

            return Ok(tariffs);
        }
    }
}
