using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_Garden_Group.Services;

namespace The_Garden_Group.Controllers;

[Authorize(Policy = "EmployeeOnly")]
public sealed class EmployeeController : Controller
{
    private readonly DashboardService _dash;

    public EmployeeController(DashboardService dash)
    {
        _dash = dash;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var vm = await _dash.GetForUserAsync(userId);

        // Force the correct view path
        return View("~/Views/Employees/Dashboard.cshtml", vm);
    }
}
