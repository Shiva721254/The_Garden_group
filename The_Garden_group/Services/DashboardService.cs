using MongoDB.Driver;
using The_Garden_Group.Models;
using The_Garden_Group.ViewModels;
using The_Garden_Group.Constants;

namespace The_Garden_Group.Services;

/// <summary>
/// Service for generating dashboard statistics.
/// </summary>
public sealed class DashboardService
{
    private readonly IMongoCollection<Ticket> _tickets;

    public DashboardService(IMongoDatabase db)
    {
        _tickets = db.GetCollection<Ticket>("tickets");
    }

    /// <summary>
    /// Gets dashboard statistics for all tickets (Service Desk view).
    /// </summary>
    public Task<DashboardVm> GetGlobalAsync()
        => BuildAsync(Builders<Ticket>.Filter.Empty);

    /// <summary>
    /// Gets dashboard statistics for tickets created by a specific user (Employee view).
    /// </summary>
    /// <param name="userId">The user's ID to filter tickets by</param>
    public Task<DashboardVm> GetForUserAsync(string userId)
        => BuildAsync(Builders<Ticket>.Filter.Eq(x => x.CreatedByUserId, userId));

    /// <summary>
    /// Builds dashboard statistics from tickets matching the filter.
    /// </summary>
    private async Task<DashboardVm> BuildAsync(FilterDefinition<Ticket> filter)
    {
        var grouped = await _tickets.Aggregate()
            .Match(filter)
            .Group(x => x.Status, g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        int open = grouped.FirstOrDefault(x => x.Status == TicketStatuses.Open)?.Count ?? 0;
        int resolved = grouped.FirstOrDefault(x => x.Status == TicketStatuses.Resolved)?.Count ?? 0;
        int closed = grouped.FirstOrDefault(x => x.Status == TicketStatuses.Closed)?.Count ?? 0;

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