using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string normalizedUsername, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}