using MediatR;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Auth.Logout
{
    public record LogoutCommand(string token, string userId) : IRequest { }
}
