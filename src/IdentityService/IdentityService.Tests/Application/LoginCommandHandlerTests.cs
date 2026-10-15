using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityService.Application.Authentication.Login;
using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Exceptions;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace IdentityService.Tests.Application;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _handler = new LoginCommandHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockJwtTokenService.Object,
            NullLogger<LoginCommandHandler>.Instance
        );
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResult()
    {
        var user = User.Create("testuser", "hashedpassword", UserRole.BiomedicalEngineer);
        var command = new LoginCommand { Username = "testuser", Password = "password" };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync("TESTUSER", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword("password", "hashedpassword"))
            .Returns(true);

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(user))
            .Returns("valid.jwt.token");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("valid.jwt.token", result.AccessToken);
        Assert.Equal("Bearer", result.TokenType);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal("BiomedicalEngineer", result.Role);
        Assert.NotEqual(DateTime.MinValue, result.ExpiresAtUtc);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsAuthenticationException()
    {
        var command = new LoginCommand { Username = "notfound", Password = "password" };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync("NOTFOUND", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<AuthenticationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public async Task Handle_InactiveUser_ThrowsAuthenticationException()
    {
        var user = User.Create("inactive", "hashedpassword", UserRole.Nurse);
        user.Deactivate();

        var command = new LoginCommand { Username = "inactive", Password = "password" };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync("INACTIVE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<AuthenticationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsAuthenticationException()
    {
        var user = User.Create("testuser", "hashedpassword", UserRole.Admin);
        var command = new LoginCommand { Username = "testuser", Password = "wrongpassword" };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync("TESTUSER", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword("wrongpassword", "hashedpassword"))
            .Returns(false);

        var ex = await Assert.ThrowsAsync<AuthenticationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Invalid username or password.", ex.Message);
    }
}