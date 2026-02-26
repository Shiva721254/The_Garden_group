using MongoDB.Driver;
using The_Garden_Group.Models;

namespace The_Garden_Group.Services;

/// <summary>
/// Service responsible for seeding the database with initial data.
/// Ensures at least 100 documents exist to meet project requirements.
/// </summary>
public sealed class SeedService
{
    private readonly IMongoCollection<Employee> _employees;
    private readonly IMongoCollection<Ticket> _tickets;

    public SeedService(IMongoDatabase db)
    {
        _employees = db.GetCollection<Employee>("employees");
        _tickets = db.GetCollection<Ticket>("tickets");
    }

    /// <summary>
    /// Seeds the database with administrative users and sample data.
    /// Creates 50 employees and 100+ tickets to meet rubric requirements.
    /// </summary>
    public async Task EnsureAdminAsync()
    {
        // 1) Ensure ServiceDesk exists
        var sdExists = await _employees.Find(x => x.Role == "serviceDesk").AnyAsync();
        if (!sdExists)
        {
            var serviceDesk = new Employee
            {
                FirstName = "ServiceDesk",
                LastName = "User",
                Email = "servicedesk@company.com",
                Role = "serviceDesk",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!")
            };
            await _employees.InsertOneAsync(serviceDesk);
        }

        // 2) Ensure Employee exists
        var empExists = await _employees.Find(x => x.Role == "employee").AnyAsync();
        if (!empExists)
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

        // 3) Seed 50 employees and 100+ tickets for rubric requirement
        await SeedLargeDatasetAsync();
    }

    /// <summary>
    /// Seeds database with 50 employees and 120 tickets to meet the "at least 100 documents" requirement.
    /// This ensures the ERD structure is properly validated with substantial data.
    /// </summary>
    private async Task SeedLargeDatasetAsync()
    {
        // Check if we already have seeded data
        var employeeCount = await _employees.CountDocumentsAsync(FilterDefinition<Employee>.Empty);
        if (employeeCount >= 50) return; // Already seeded

        var employees = new List<Employee>();
        var random = new Random(42); // Fixed seed for reproducibility

        // Generate 48 additional employees (we already have 2)
        var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emily", "Robert", "Lisa", "James", "Mary", "William", "Patricia", "Richard", "Jennifer", "Thomas", "Linda", "Charles", "Barbara", "Daniel", "Elizabeth", "Matthew", "Susan", "Anthony", "Jessica", "Mark", "Karen" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Thompson", "White", "Harris" };

        for (int i = 1; i <= 48; i++)
        {
            var employee = new Employee
            {
                FirstName = firstNames[random.Next(firstNames.Length)],
                LastName = lastNames[random.Next(lastNames.Length)],
                Email = $"employee{i}@company.com",
                Role = i <= 5 ? "serviceDesk" : "employee", // 5 service desk, rest employees
                IsActive = i <= 45, // 3 inactive for testing
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!")
            };
            employees.Add(employee);
        }

        if (employees.Count > 0)
        {
            await _employees.InsertManyAsync(employees);
        }

        // Now seed 120 tickets
        var allEmployees = await _employees.Find(x => x.Role == "employee").ToListAsync();
        if (allEmployees.Count == 0) return;

        var tickets = new List<Ticket>();
        var subjects = new[]
        {
            "Unable to login to email",
            "Password reset required",
            "VPN connection issues",
            "Software installation request",
            "Printer not working",
            "Network connectivity problem",
            "Computer running slow",
            "Email not sending",
            "Application crash on startup",
            "Access denied to shared folder",
            "Monitor display issues",
            "Keyboard malfunction",
            "Mouse not responding",
            "Laptop overheating",
            "Browser freezing frequently",
            "Cannot access database",
            "File recovery needed",
            "Software license expired",
            "New employee account setup",
            "Phone not working",
            "Video conferencing issues",
            "Disk space running low",
            "Antivirus update failed",
            "Remote desktop not connecting"
        };

        var descriptions = new[]
        {
            "This issue started this morning and is blocking my work.",
            "I've tried restarting but the problem persists.",
            "This is affecting my productivity significantly.",
            "Need urgent assistance with this matter.",
            "Problem occurs intermittently throughout the day.",
            "Error message appears when attempting to perform this action.",
            "Have tried basic troubleshooting steps without success.",
            "This is a recurring issue that needs permanent solution.",
            "Unable to complete my tasks due to this problem.",
            "Colleagues are experiencing the same issue.",
            "System becomes unresponsive when this happens.",
            "Lost important data due to this malfunction.",
            "Cannot access critical files for my project.",
            "Performance degradation noticed over past week.",
            "Need immediate support to resolve this."
        };

        var categories = new[] { "Hardware", "Software", "Network", "Access", "Email", "Other" };
        var priorities = new[] { "low", "medium", "high" };
        var statuses = new[] { "open", "resolved", "closed" };

        for (int i = 0; i < 120; i++)
        {
            var creator = allEmployees[random.Next(allEmployees.Count)];
            var createdDate = DateTime.UtcNow.AddDays(-random.Next(1, 180));
            var status = statuses[random.Next(statuses.Length)];

            var ticket = new Ticket
            {
                Subject = subjects[random.Next(subjects.Length)],
                Description = descriptions[random.Next(descriptions.Length)],
                Category = categories[random.Next(categories.Length)],
                Priority = priorities[random.Next(priorities.Length)],
                Status = status,
                CreatedByUserId = creator.Id ?? "",
                CreatedAt = createdDate,
                UpdatedAt = createdDate.AddHours(random.Next(1, 48)),
                ResolutionNote = status == "resolved" ? "Issue resolved successfully." : null,
                ResolvedAt = status == "resolved" ? createdDate.AddHours(random.Next(1, 72)) : null
            };

            tickets.Add(ticket);
        }

        if (tickets.Count > 0)
        {
            await _tickets.InsertManyAsync(tickets);
        }
    }
}
