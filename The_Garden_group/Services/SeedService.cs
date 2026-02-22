using MongoDB.Driver;
using The_Garden_Group.Models;

namespace The_Garden_Group.Services;

public sealed class SeedService
{
    private readonly IMongoCollection<Employee> _employees;

    public SeedService(IMongoDatabase db)
    {
        _employees = db.GetCollection<Employee>("employees");
    }

    public async Task EnsureAdminAsync()
    {
        // 1) Ensure at least 1 ServiceDesk admin exists
        var adminExists = await _employees.Find(x => x.Role == "serviceDesk").AnyAsync();
        if (!adminExists)
        {
            var admin = new Employee
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@company.com",
                Role = "serviceDesk",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!")
            };

            await _employees.InsertOneAsync(admin);
        }

        // 2) Ensure at least 1 regular employee exists
        var employeeExists = await _employees.Find(x => x.Role == "employee").AnyAsync();
        if (!employeeExists)
        {
            var emp = new Employee
            {
                FirstName = "Test",
                LastName = "Employee",
                Email = "employee@company.com",
                Role = "employee",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!")
            };

            await _employees.InsertOneAsync(emp);
        }
    }







}