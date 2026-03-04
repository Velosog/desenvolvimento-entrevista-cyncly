using ErpDemo.Domain.Entities;

namespace ErpDemo.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(
        string? name, string? document, bool? isActive, int page, int pageSize);
    Task<Customer> AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null);
}
