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

    public async Task<IActionResult> Dashboard()
    {
        var vm = await _dash.GetGlobalAsync();
        return View(vm);
    }
}