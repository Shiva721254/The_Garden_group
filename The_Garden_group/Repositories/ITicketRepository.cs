
using The_Garden_Group.Models;

namespace The_Garden_Group.Repositories;

/// <summary>
/// Repository interface for Ticket data access operations.
/// Provides CRUD operations and query methods for tickets in MongoDB.
/// </summary>
public interface ITicketRepository
{
    /// <summary>
    /// Retrieves a single ticket by its unique identifier.
    /// </summary>
    /// <param name="id">The MongoDB ObjectId of the ticket as a string</param>
    /// <returns>The ticket if found, otherwise null</returns>
    Task<Ticket?> GetByIdAsync(string id);

    /// <summary>
    /// Retrieves all tickets in the system.
    /// Used by service desk to view all tickets.
    /// </summary>
    /// <returns>A list of all tickets</returns>
    Task<List<Ticket>> GetAllAsync();

    /// <summary>
    /// Retrieves all tickets created by a specific user.
    /// Used for employee dashboard to show their own tickets.
    /// </summary>
    /// <param name="userId">The MongoDB ObjectId of the user as a string</param>
    /// <returns>A list of tickets created by the specified user</returns>
    Task<List<Ticket>> GetByCreatorAsync(string userId);

    /// <summary>
    /// Creates a new ticket in the database.
    /// Automatically assigns a new ObjectId to the ticket.
    /// </summary>
    /// <param name="ticket">The ticket object to create</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task CreateAsync(Ticket ticket);

    /// <summary>
    /// Updates an existing ticket in the database.
    /// Updates all fields including status, priority, and resolution notes.
    /// </summary>
    /// <param name="ticket">The ticket object with updated values</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateAsync(Ticket ticket);

    /// <summary>
    /// Permanently deletes a ticket from the database.
    /// This operation cannot be undone.
    /// </summary>
    /// <param name="id">The MongoDB ObjectId of the ticket to delete as a string</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteAsync(string id);
}
