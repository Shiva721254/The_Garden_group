using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_Garden_Group.Models;
using The_Garden_Group.Repositories;
using The_Garden_Group.ViewModels;

namespace The_Garden_Group.Controllers;

[Authorize(Policy = "ServiceDeskOnly")]
[Route("Employees")]
public sealed class EmployeesController : Controller
{
    private readonly IEmployeeRepository _employees;

    public EmployeesController(IEmployeeRepository employees)
    {
        _employees = employees;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
        => View(await _employees.GetAllAsync());

    [HttpGet("Create")]
    public IActionResult Create() => View(new EmployeeFormVm());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormVm vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Password))
            ModelState.AddModelError(nameof(vm.Password), "Password is required.");

        var allowedRoles = new[] { "employee", "serviceDesk" };
        if (!allowedRoles.Contains(vm.Role))
            ModelState.AddModelError(nameof(vm.Role), "Invalid role.");

        if (!ModelState.IsValid) return View(vm);

        var model = new Employee
        {
            FirstName = vm.FirstName.Trim(),
            LastName = vm.LastName.Trim(),
            Email = vm.Email.Trim().ToLowerInvariant(),
            Role = vm.Role,
            IsActive = vm.IsActive,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _employees.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var emp = await _employees.GetByIdAsync(id);
        if (emp is null) return NotFound();

        return View(new EmployeeFormVm
        {
            Id = emp.Id,
            FirstName = emp.FirstName,
            LastName = emp.LastName,
            Email = emp.Email,
            Role = emp.Role,
            IsActive = emp.IsActive
        });
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EmployeeFormVm vm)
    {
        var allowedRoles = new[] { "employee", "serviceDesk" };
        if (!allowedRoles.Contains(vm.Role))
            ModelState.AddModelError(nameof(vm.Role), "Invalid role.");

        if (!ModelState.IsValid) return View(vm);

        var existing = await _employees.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.FirstName = vm.FirstName.Trim();
        existing.LastName = vm.LastName.Trim();
        existing.Email = vm.Email.Trim().ToLowerInvariant();
        existing.Role = vm.Role;
        existing.IsActive = vm.IsActive;

        if (!string.IsNullOrWhiteSpace(vm.Password))
            existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password);

        await _employees.UpdateAsync(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var emp = await _employees.GetByIdAsync(id);
        if (emp is null) return NotFound();
        return View(emp);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, string? _ = null)
    {
        await _employees.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}