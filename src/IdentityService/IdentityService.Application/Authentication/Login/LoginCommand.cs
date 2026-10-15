using MediatR;

namespace IdentityService.Application.Authentication.Login;

public sealed class LoginCommand : IRequest<LoginResult>
{
    public string Username { get; init; }
    public string Password { get; init; }
}