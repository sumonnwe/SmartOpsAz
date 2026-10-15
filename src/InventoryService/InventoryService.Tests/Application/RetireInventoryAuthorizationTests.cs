using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Inventory.RetireInventory;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace InventoryService.Tests.Application;

public class RetireInventoryAuthorizationTests
{
    private readonly string _signingKey = "super-secret-key-that-is-at-least-32-characters-long!";

    [Fact]
    public void GenerateTestToken_WithBiomedicalEngineerRole_GeneratesValidJwt()
    {
        var token = GenerateTestToken("BiomedicalEngineer");

        Assert.NotEmpty(token);
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadToken(token) as JwtSecurityToken;

        Assert.NotNull(decodedToken);
        var roleClaim = decodedToken?.Claims.FirstOrDefault(c => c.Type == "role");
        Assert.NotNull(roleClaim);
        Assert.Equal("BiomedicalEngineer", roleClaim!.Value);
    }

    [Fact]
    public void GenerateTestToken_WithNurseRole_GeneratesValidJwt()
    {
        var token = GenerateTestToken("Nurse");

        Assert.NotEmpty(token);
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadToken(token) as JwtSecurityToken;

        Assert.NotNull(decodedToken);
        var roleClaim = decodedToken?.Claims.FirstOrDefault(c => c.Type == "role");
        Assert.NotNull(roleClaim);
        Assert.Equal("Nurse", roleClaim!.Value);
    }

    private string GenerateTestToken(string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, "testuser"),
            new("role", role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "SmartInventory.Identity",
            audience: "SmartInventory.Api",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}