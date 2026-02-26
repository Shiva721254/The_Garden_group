using System.ComponentModel.DataAnnotations;

namespace The_Garden_Group.ViewModels;

public sealed class EmployeeFormVm
{
    public string? Id { get; set; }

    [Required, StringLength(40)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(40)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Role { get; set; } = "employee"; // employee | serviceDesk | admin (if you use admin)

    public bool IsActive { get; set; } = true;

    // Create: required (controller enforces)
    // Edit: optional (leave empty to keep current password)
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string? Password { get; set; }
}