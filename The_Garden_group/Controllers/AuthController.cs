using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using The_Garden_Group.Services;
using The_Garden_Group.ViewModels;
using The_Garden_Group.Constants;

namespace The_Garden_Group.Controllers;

public sealed class AuthController : Controller
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToDashboard(User.FindFirst(ClaimTypes.Role)?.Value);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _auth.ValidateLoginAsync(vm.Email, vm.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToDashboard(user.Role);
    }

    [HttpPost("/Auth/Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("The_Garden_Group.Auth");
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Denied() => View();

    /// <summary>
    /// Redirects user to appropriate dashboard based on role.
    /// </summary>
    private IActionResult RedirectToDashboard(string? role) =>
        role == Roles.ServiceDesk
            ? RedirectToAction("Dashboard", "ServiceDesk")
            : RedirectToAction("Dashboard", "Employee");
}