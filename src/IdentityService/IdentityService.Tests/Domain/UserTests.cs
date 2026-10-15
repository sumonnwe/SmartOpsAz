using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using Xunit;

namespace IdentityService.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_SucceedsAndCreatesActiveUser()
    {
        var username = "testuser";
        var passwordHash = "hashedpassword";
        var role = UserRole.BiomedicalEngineer;

        var user = User.Create(username, passwordHash, role);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal(username.ToUpperInvariant(), user.NormalizedUsername);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(role, user.Role);
        Assert.True(user.IsActive);
        Assert.NotEqual(DateTime.MinValue, user.CreatedAt);
    }

    [Fact]
    public void Create_WithEmptyUsername_Throws()
    {
        Assert.Throws<ArgumentException>(() => 
            User.Create("", "hash", UserRole.Admin));
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_Throws()
    {
        Assert.Throws<ArgumentException>(() => 
            User.Create("testuser", "", UserRole.Admin));
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var user = User.Create("testuser", "hash", UserRole.Nurse);

        user.Deactivate();

        Assert.False(user.IsActive);
    }
}   