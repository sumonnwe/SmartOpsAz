namespace IdentityService.Contracts.Authentication;

public sealed class LoginRequest
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}