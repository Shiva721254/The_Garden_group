using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using The_Garden_Group.Services;
using The_Garden_Group.ViewModels;

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
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return role == "serviceDesk"
                ? RedirectToAction("Dashboard", "ServiceDesk")
                : RedirectToAction("Dashboard", "Employee");
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
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
    new Claim(ClaimTypes.Role, user.Role) // IMPORTANT
};

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return user.Role == "serviceDesk"
            ? RedirectToAction("Dashboard", "ServiceDesk")
            : RedirectToAction("Dashboard", "Employee");
    }

    [HttpPost("/Auth/Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Extra safety: remove the cookie by name too
        Response.Cookies.Delete("The_Garden_Group.Auth");

        return RedirectToAction("Login", "Auth");
    }

    public IActionResult Denied() => View();
}