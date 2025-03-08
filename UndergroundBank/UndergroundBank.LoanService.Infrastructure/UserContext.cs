using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UndergroundBank.LoanService.Infrastructure
{
    public interface IUserContext
    {
        Guid UserId { get; }
    }

    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(
                    ClaimTypes.NameIdentifier
                );
                return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
            }
        }
    }
}
