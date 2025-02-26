using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UndergroundBank.Common.Base
{
    [Route("/api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IMediator Mediator;

        protected BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
