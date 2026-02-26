using MongoDB.Driver;
using The_Garden_Group.Models;
using The_Garden_Group.ViewModels;

namespace The_Garden_Group.Services;

public sealed class DashboardService
{
    private readonly IMongoCollection<Ticket> _tickets;

    public DashboardService(IMongoDatabase db)
    {
        _tickets = db.GetCollection<Ticket>("tickets");
    }

    // ServiceDesk: all tickets (global)
    public Task<DashboardVm> GetGlobalAsync()
        => BuildAsync(Builders<Ticket>.Filter.Empty);

    // Employee: only own tickets
    public Task<DashboardVm> GetForUserAsync(string userId)
        => BuildAsync(Builders<Ticket>.Filter.Eq(x => x.CreatedByUserId, userId));

    private async Task<DashboardVm> BuildAsync(FilterDefinition<Ticket> filter)
    {
        var grouped = await _tickets.Aggregate()
            .Match(filter)
            .Group(x => x.Status, g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        int open = grouped.FirstOrDefault(x => x.Status == "open")?.Count ?? 0;
        int resolved = grouped.FirstOrDefault(x => x.Status == "resolved")?.Count ?? 0;
        int closed = grouped.FirstOrDefault(x => x.Status == "closed")?.Count ?? 0;

        int total = open + resolved + closed;

        return new DashboardVm
        {
            Open = open,
            Resolved = resolved,
            Closed = closed,
            Total = total
        };
    }
}