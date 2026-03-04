using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;
using ErpDemo.Domain.Enums;

namespace ErpDemo.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> GetByIdAsync(Guid id);
    Task<PagedResult<OrderDto>> GetPagedAsync(Guid? customerId, OrderStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto);
    Task<OrderItemDto> AddItemAsync(Guid orderId, CreateOrderItemDto dto);
    Task<OrderItemDto> UpdateItemAsync(Guid orderId, Guid itemId, UpdateOrderItemDto dto);
    Task RemoveItemAsync(Guid orderId, Guid itemId);
    Task DeleteAsync(Guid id);
}
