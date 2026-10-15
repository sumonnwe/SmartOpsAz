using System.Threading;
using System.Threading.Tasks;
using IdentityService.Application.Authentication.Login;
using IdentityService.Contracts.Authentication;
using IdentityService.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ISender sender, ILogger<AuthController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Login with username and password to receive JWT.
    /// </summary>
    /// <response code="200">Login successful, JWT returned</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login request for username: {Username}", request.Username);

        var command = new LoginCommand
        {
            Username = request.Username,
            Password = request.Password
        };

        try
        {
            var result = await _sender.Send(command, cancellationToken);

            var response = new LoginResponse
            {
                AccessToken = result.AccessToken,
                TokenType = result.TokenType,
                ExpiresAtUtc = result.ExpiresAtUtc,
                UserId = result.UserId,
                Username = result.Username,
                Role = result.Role
            };

            return Ok(response);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogWarning("Authentication failed: {Message}", ex.Message);
            return Unauthorized(new { error = ex.Message });
        }
    }
}