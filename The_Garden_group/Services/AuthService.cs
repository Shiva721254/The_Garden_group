using The_Garden_Group.Models;
using The_Garden_Group.Repositories;

namespace The_Garden_Group.Services;

public sealed class AuthService
{
    private readonly IEmployeeRepository _employees;

    public AuthService(IEmployeeRepository employees)
    {
        _employees = employees;
    }

    public async Task<Employee?> ValidateLoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _employees.GetByEmailAsync(email.Trim().ToLower());
        if (user is null) return null;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }
}