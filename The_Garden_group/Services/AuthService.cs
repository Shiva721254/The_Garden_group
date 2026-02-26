using The_Garden_Group.Models;
using The_Garden_Group.Repositories;

namespace The_Garden_Group.Services;

/// <summary>
/// Service for authentication operations.
/// </summary>
public sealed class AuthService
{
    private readonly IEmployeeRepository _employees;

    public AuthService(IEmployeeRepository employees)
    {
        _employees = employees;
    }

    /// <summary>
    /// Validates user login credentials.
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="password">Plain text password</param>
    /// <returns>Employee if credentials are valid, otherwise null</returns>
    public async Task<Employee?> ValidateLoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _employees.GetByEmailAsync(normalizedEmail);

        if (user is null || !user.IsActive)
            return null;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }
}