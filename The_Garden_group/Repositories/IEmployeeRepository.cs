
using The_Garden_Group.Models;

namespace The_Garden_Group.Repositories;


public interface IEmployeeRepository
{
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByIdAsync(string id);
    Task<List<Employee>> GetAllAsync();
    Task CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(string id);
}