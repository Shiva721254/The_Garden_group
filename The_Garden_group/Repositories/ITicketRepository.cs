
using The_Garden_Group.Models;

namespace The_Garden_Group.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(string id);
    Task<List<Ticket>> GetAllAsync();
    Task<List<Ticket>> GetByCreatorAsync(string userId);
    Task CreateAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
    Task DeleteAsync(string id);
}