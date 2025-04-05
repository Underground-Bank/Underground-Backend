using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.BankAccountService.Web.Controllers
{
    [ApiController]
    [Route("api/currency")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrencyService _currencyService;

        public CurrencyController(
            ILogger<CurrencyController> logger,
            ICurrencyService currencyService
        )
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        [Authorize(Roles = $"{nameof(Role.Admin)}")]
        [HttpGet("update")]
        public async Task<IActionResult> GetAll()
        {
            await _currencyService.GetCurrency();
            return Ok();
        }
    }
}
