using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        protected Guid UserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
            }
        }
    }
}
