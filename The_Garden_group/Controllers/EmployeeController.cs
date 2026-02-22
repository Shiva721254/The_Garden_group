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

    public async Task<IActionResult> Dashboard()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var vm = await _dash.GetForUserAsync(userId);
        return View(vm);
    }
}