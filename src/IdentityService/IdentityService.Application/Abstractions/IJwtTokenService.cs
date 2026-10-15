using System.Threading.Tasks;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}