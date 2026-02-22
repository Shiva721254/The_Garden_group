using System.ComponentModel.DataAnnotations;

namespace The_Garden_Group.ViewModels;

public sealed class LoginVm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}