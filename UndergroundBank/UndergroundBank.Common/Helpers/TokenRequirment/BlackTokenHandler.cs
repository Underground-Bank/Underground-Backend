using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UndergroundBank.Common.Data;

namespace UndergroundBank.Common.Helpers.TokenRequirment
{
    public class BlackTokenHandler : AuthorizationHandler<TokenBlackListRequirment>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BlackTokenHandler> _logger;

        public BlackTokenHandler(
            IServiceProvider serviceProvider,
            ILogger<BlackTokenHandler> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TokenBlackListRequirment requirement
        )
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<RedisDbContext>();
                    var httpContextAccessor =
                        _serviceProvider.GetRequiredService<IHttpContextAccessor>();

                    if (httpContextAccessor.HttpContext == null)
                    {
                        _logger.LogError("HttpContext is null in BlackTokenHandler");
                        context.Fail();
                        return;
                    }

                    var authorizationHeader = httpContextAccessor
                        .HttpContext.Request.Headers["Authorization"]
                        .FirstOrDefault();

                    if (
                        string.IsNullOrEmpty(authorizationHeader)
                        || !authorizationHeader.StartsWith("Bearer ")
                    )
                    {
                        _logger.LogWarning("Authorization header is missing or invalid.");
                        httpContextAccessor.HttpContext.Response.StatusCode = (int)
                            HttpStatusCode.Unauthorized;
                        context.Fail();
                        return;
                    }

                    var token = authorizationHeader.Substring("Bearer ".Length);
                    var blacklisted = await db.IsBlackToken(token);

                    if (blacklisted)
                    {
                        _logger.LogWarning("Token is blacklisted.");
                        httpContextAccessor.HttpContext.Response.StatusCode = (int)
                            HttpStatusCode.Unauthorized;
                        context.Fail();
                        return;
                    }

                    context.Succeed(requirement);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in BlackTokenHandler.");
                    context.Fail();

                    var httpContextAccessor =
                        _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    if (httpContextAccessor.HttpContext != null)
                    {
                        httpContextAccessor.HttpContext.Response.StatusCode = (int)
                            HttpStatusCode.InternalServerError;
                    }
                }
            }
        }
    }
}
