using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityService.Application.Abstractions;
using IdentityService.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Authentication.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for username: {Username}", command.Username);

        var normalizedUsername = command.Username.Trim().ToUpperInvariant();
        var user = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);

        if (user is null || !user.IsActive || !_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid login attempt for username: {Username}", command.Username);
            throw new AuthenticationException("Invalid username or password.");
        }

        var token = _jwtTokenService.GenerateToken(user);
        var expirationMinutes = 60;

        return new LoginResult
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes),
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }
}