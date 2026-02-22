using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_Garden_Group.Models;
using The_Garden_Group.Repositories;

namespace The_Garden_Group.Controllers;

[Authorize(Policy = "ServiceDeskOnly")]
[Route("AdminEmployees")]
public sealed class AdminEmployeesController : Controller
{
    private readonly IEmployeeRepository _employees;

    public AdminEmployeesController(IEmployeeRepository employees)
    {
        _employees = employees;
    }

    // GET /AdminEmployees
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var list = await _employees.GetAllAsync();
        return View(list);
    }

    // GET /AdminEmployees/Create
    [HttpGet("Create")]
    public IActionResult Create() => View(new Employee());

    // POST /AdminEmployees/Create
    [HttpPost("Create")]
    public async Task<IActionResult> Create(Employee model, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            ModelState.AddModelError("", "Password is required.");

        if (!ModelState.IsValid) return View(model);

        model.Email = model.Email.Trim().ToLower();
        model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        model.CreatedAt = DateTime.UtcNow;
        model.IsActive = true;

        await _employees.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    // GET /AdminEmployees/Edit/{id}
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var emp = await _employees.GetByIdAsync(id);
        if (emp is null) return NotFound();
        return View(emp);
    }

    // POST /AdminEmployees/Edit
    [HttpPost("Edit")]
    public async Task<IActionResult> Edit(Employee model)
    {
        if (!ModelState.IsValid) return View(model);

        var existing = await _employees.GetByIdAsync(model.Id!);
        if (existing is null) return NotFound();

        existing.FirstName = model.FirstName;
        existing.LastName = model.LastName;
        existing.Email = model.Email.Trim().ToLower();
        existing.Role = model.Role;
        existing.IsActive = model.IsActive;

        await _employees.UpdateAsync(existing);
        return RedirectToAction(nameof(Index));
    }

    // POST /AdminEmployees/Delete/{id}
    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _employees.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}