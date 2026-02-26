using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_Garden_Group.Models;
using The_Garden_Group.Repositories;
using The_Garden_Group.ViewModels;
using The_Garden_Group.Services;
using The_Garden_Group.Constants;

namespace The_Garden_Group.Controllers;

[Authorize]
[Route("Tickets")]
public sealed class TicketsController : Controller
{
    private readonly ITicketRepository _tickets;
    private readonly TicketSearchService _search;

    public TicketsController(ITicketRepository tickets, TicketSearchService search)
    {
        _tickets = tickets;
        _search = search;
    }

    // ============================
    // EMPLOYEE
    // ============================

    // GET /Tickets/Create
    [Authorize(Policy = "EmployeeOnly")]
    [HttpGet("Create")]
    public IActionResult Create() => View(new TicketCreateVm());

    // POST /Tickets/Create
    [Authorize(Policy = "EmployeeOnly")]
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TicketCreateVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var ticket = new Ticket
        {
            Subject = vm.Subject.Trim(),
            Description = vm.Description.Trim(),
            Category = vm.Category,
            Priority = vm.Priority,
            Status = TicketStatuses.Open,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _tickets.CreateAsync(ticket);
        return RedirectToAction(nameof(MyTickets));
    }

    // GET /Tickets/MyTickets
    [Authorize(Policy = "EmployeeOnly")]
    [HttpGet("MyTickets")]
    public async Task<IActionResult> MyTickets()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var list = await _tickets.GetByCreatorAsync(userId);
        return View(list);
    }

    // ============================
    // SERVICE DESK
    // ============================

    // GET /Tickets?query=...&mode=AND|OR
    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("")]
    public async Task<IActionResult> Index(string? query, string mode = "OR")
    {
        ViewBag.Query = query ?? string.Empty;
        ViewBag.Mode = mode;

        var list = await _search.SearchAsync(query, mode);
        return View(list);
    }

    // GET /Tickets/Edit/{id}
    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var t = await _tickets.GetByIdAsync(id);
        if (t is null) return NotFound();

        var vm = new TicketEditVm
        {
            Id = t.Id ?? string.Empty,
            Subject = t.Subject,
            Description = t.Description,
            Category = t.Category,
            Priority = t.Priority,
            Status = t.Status,
            ResolutionNote = t.ResolutionNote
        };

        return View(vm);
    }

    // POST /Tickets/Edit
    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpPost("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TicketEditVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var t = await _tickets.GetByIdAsync(vm.Id);
        if (t is null) return NotFound();

        t.Subject = vm.Subject.Trim();
        t.Description = vm.Description.Trim();
        t.Category = vm.Category;
        t.Priority = vm.Priority;
        t.Status = vm.Status;

        // Status-specific logic
        switch (t.Status)
        {
            case TicketStatuses.Resolved:
                t.ResolutionNote = string.IsNullOrWhiteSpace(vm.ResolutionNote) 
                    ? "Resolved" 
                    : vm.ResolutionNote.Trim();
                t.ResolvedAt ??= DateTime.UtcNow;
                break;

            case TicketStatuses.Closed when string.IsNullOrWhiteSpace(t.ResolutionNote):
                t.ResolutionNote = "Closed without resolution";
                break;

            case TicketStatuses.Open:
                t.ResolutionNote = null;
                t.ResolvedAt = null;
                break;
        }

        t.UpdatedAt = DateTime.UtcNow;

        await _tickets.UpdateAsync(t);
        return RedirectToAction(nameof(Index));
    }

    // POST /Tickets/Delete/{id}
    [Authorize(Policy = "ServiceDeskOnly")]
    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _tickets.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}