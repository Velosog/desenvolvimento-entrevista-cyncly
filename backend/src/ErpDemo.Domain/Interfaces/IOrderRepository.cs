using ErpDemo.Domain.Entities;
using ErpDemo.Domain.Enums;

namespace ErpDemo.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
        Guid? customerId, OrderStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize);
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
