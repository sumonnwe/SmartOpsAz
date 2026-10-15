using System;
using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string NormalizedUsername { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    private User(Guid id, string username, string normalizedUsername, string passwordHash, UserRole role)
    {
        Id = id;
        Username = username;
        NormalizedUsername = normalizedUsername;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(string username, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        return new User(
            Guid.NewGuid(),
            username.Trim(),
            username.Trim().ToUpperInvariant(),
            passwordHash,
            role
        );
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public static User Hydrate(Guid id, string username, string normalizedUsername, string passwordHash, UserRole role, bool isActive, DateTime createdAt)
    {
        var user = new User();
        user.Id = id;
        user.Username = username;
        user.NormalizedUsername = normalizedUsername;
        user.PasswordHash = passwordHash;
        user.Role = role;
        user.IsActive = isActive;
        user.CreatedAt = createdAt;
        return user;
    }
}