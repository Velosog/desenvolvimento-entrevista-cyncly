using ErpDemo.Domain.Entities;

namespace ErpDemo.Domain.Interfaces;

public interface IOrderItemRepository
{
    Task<OrderItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
    Task<OrderItem> AddAsync(OrderItem item);
    Task UpdateAsync(OrderItem item);
    Task DeleteAsync(Guid id);
}
