using System.ComponentModel.DataAnnotations;

namespace The_Garden_Group.ViewModels;

public sealed class TicketCreateVm
{
    [Required, StringLength(100)]
    public string Subject { get; set; } = "";

    [Required, StringLength(2000)]
    public string Description { get; set; } = "";

    [Required]
    public string Category { get; set; } = "other";

    [Required]
    public string Priority { get; set; } = "medium";
}