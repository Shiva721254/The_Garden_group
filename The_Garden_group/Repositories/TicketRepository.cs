using MongoDB.Driver;
using The_Garden_Group.Models;

namespace The_Garden_Group.Repositories;

public sealed class TicketRepository : ITicketRepository
{
    private readonly IMongoCollection<Ticket> _tickets;

    public TicketRepository(IMongoDatabase db)
    {
        _tickets = db.GetCollection<Ticket>("tickets");
    }

    public Task<Ticket?> GetByIdAsync(string id) =>
        _tickets.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<List<Ticket>> GetAllAsync() =>
        _tickets.Find(_ => true).SortByDescending(x => x.CreatedAt).ToListAsync();

    public Task<List<Ticket>> GetByCreatorAsync(string userId) =>
        _tickets.Find(x => x.CreatedByUserId == userId)
               .SortByDescending(x => x.CreatedAt)
               .ToListAsync();

    public Task CreateAsync(Ticket ticket) =>
        _tickets.InsertOneAsync(ticket);

    public Task UpdateAsync(Ticket ticket) =>
        _tickets.ReplaceOneAsync(x => x.Id == ticket.Id, ticket);

    public Task DeleteAsync(string id) =>
        _tickets.DeleteOneAsync(x => x.Id == id);
}