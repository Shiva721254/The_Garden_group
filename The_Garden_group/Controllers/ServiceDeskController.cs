using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_Garden_Group.Services;

namespace The_Garden_Group.Controllers;

[Authorize(Policy = "ServiceDeskOnly")]
public sealed class ServiceDeskController : Controller
{
    private readonly DashboardService _dash;

    public ServiceDeskController(DashboardService dash)
    {
        _dash = dash;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var vm = await _dash.GetGlobalAsync();

        // Force the correct view (prevents accidentally showing Employee/Dashboard)
        return View("~/Views/ServiceDesk/Dashboard.cshtml", vm);
    }
}