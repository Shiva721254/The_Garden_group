
using The_Garden_Group.Models;

namespace The_Garden_Group.Repositories;

/// <summary>
/// Repository interface for Employee data access operations.
/// Provides CRUD operations and query methods for employees in MongoDB.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// Retrieves an employee by their email address.
    /// Email is case-insensitive and used for authentication.
    /// </summary>
    /// <param name="email">The employee's email address</param>
    /// <returns>The employee if found, otherwise null</returns>
    Task<Employee?> GetByEmailAsync(string email);

    /// <summary>
    /// Retrieves an employee by their unique identifier.
    /// </summary>
    /// <param name="id">The MongoDB ObjectId of the employee as a string</param>
    /// <returns>The employee if found, otherwise null</returns>
    Task<Employee?> GetByIdAsync(string id);

    /// <summary>
    /// Retrieves all employees in the system.
    /// Used by service desk for employee management.
    /// </summary>
    /// <returns>A list of all employees</returns>
    Task<List<Employee>> GetAllAsync();

    /// <summary>
    /// Creates a new employee in the database.
    /// Email must be unique. Password should be hashed before calling this method.
    /// </summary>
    /// <param name="employee">The employee object to create</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task CreateAsync(Employee employee);

    /// <summary>
    /// Updates an existing employee in the database.
    /// Updates all fields including role and active status.
    /// </summary>
    /// <param name="employee">The employee object with updated values</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateAsync(Employee employee);

    /// <summary>
    /// Permanently deletes an employee from the database.
    /// This operation cannot be undone.
    /// </summary>
    /// <param name="id">The MongoDB ObjectId of the employee to delete as a string</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteAsync(string id);
}
