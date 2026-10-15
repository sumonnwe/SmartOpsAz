using IdentityService.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Authentication;

public sealed class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hash, password);
        return result != PasswordVerificationResult.Failed;
    }
}