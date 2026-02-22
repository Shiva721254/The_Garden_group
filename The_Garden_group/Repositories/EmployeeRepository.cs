using MongoDB.Driver;
using The_Garden_Group.Models;
using The_Garden_Group.Repositories;

namespace The_Garden_Group.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly IMongoCollection<Employee> _employees;

    public EmployeeRepository(IMongoDatabase db)
    {
        _employees = db.GetCollection<Employee>("employees");
    }

    public Task<Employee?> GetByEmailAsync(string email) =>
        _employees.Find(x => x.Email == email && x.IsActive).FirstOrDefaultAsync();

    public Task<Employee?> GetByIdAsync(string id) =>
        _employees.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<List<Employee>> GetAllAsync() =>
        _employees.Find(_ => true).ToListAsync();

    public Task CreateAsync(Employee employee) =>
        _employees.InsertOneAsync(employee);

    public Task UpdateAsync(Employee employee) =>
        _employees.ReplaceOneAsync(x => x.Id == employee.Id, employee);

    public Task DeleteAsync(string id) =>
        _employees.DeleteOneAsync(x => x.Id == id);
}