using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UndergroundBank.Common.Data.Enums;

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

        protected List<Role> Roles
        {
            get
            {
                return User
                    .Claims.Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => Enum.TryParse<Role>(c.Value, out var role) ? role : default)
                    .Where(role => role != default)
                    .ToList();
            }
        }
    }
}
