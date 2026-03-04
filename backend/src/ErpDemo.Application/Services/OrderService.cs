using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using ErpDemo.Domain.Entities;
using ErpDemo.Domain.Enums;
using ErpDemo.Domain.Interfaces;

namespace ErpDemo.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _itemRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderItemRepository itemRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _itemRepository = itemRepository;
        _customerRepository = customerRepository;
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<PagedResult<OrderDto>> GetPagedAsync(
        Guid? customerId, OrderStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
    {
        var (items, totalCount) = await _orderRepository.GetPagedAsync(customerId, status, fromDate, toDate, page, pageSize);
        return new PagedResult<OrderDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {
        if (!await _customerRepository.ExistsAsync(dto.CustomerId))
            throw new KeyNotFoundException("Cliente não encontrado.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            Status = OrderStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            Items = dto.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        order.RecalculateTotal();
        await _orderRepository.AddAsync(order);

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Pedido não encontrado.");

        if (order.Status == OrderStatus.Canceled)
            throw new InvalidOperationException("Pedido cancelado não pode ser alterado.");

        if (order.Status == OrderStatus.Confirmed && dto.Status == OrderStatus.Draft)
            throw new InvalidOperationException("Pedido confirmado não pode voltar para rascunho.");

        order.Status = dto.Status;
        await _orderRepository.UpdateAsync(order);
        return MapToDto(order);
    }

    public async Task<OrderItemDto> AddItemAsync(Guid orderId, CreateOrderItemDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException("Pedido não encontrado.");

        if (order.Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("Não é possível editar itens de um pedido confirmado.");

        if (order.Status == OrderStatus.Canceled)
            throw new InvalidOperationException("Pedido cancelado não pode ser alterado.");

        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Description = dto.Description,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice
        };

        await _itemRepository.AddAsync(item);

        order.Items.Add(item);
        order.RecalculateTotal();
        await _orderRepository.UpdateAsync(order);

        return MapItemToDto(item);
    }

    public async Task<OrderItemDto> UpdateItemAsync(Guid orderId, Guid itemId, UpdateOrderItemDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException("Pedido não encontrado.");

        if (order.Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("Não é possível editar itens de um pedido confirmado.");

        if (order.Status == OrderStatus.Canceled)
            throw new InvalidOperationException("Pedido cancelado não pode ser alterado.");

        var item = await _itemRepository.GetByIdAsync(itemId)
            ?? throw new KeyNotFoundException("Item não encontrado.");

        if (item.OrderId != orderId)
            throw new InvalidOperationException("Item não pertence a este pedido.");

        item.Description = dto.Description;
        item.Quantity = dto.Quantity;
        item.UnitPrice = dto.UnitPrice;

        await _itemRepository.UpdateAsync(item);

        order.RecalculateTotal();
        await _orderRepository.UpdateAsync(order);

        return MapItemToDto(item);
    }

    public async Task RemoveItemAsync(Guid orderId, Guid itemId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException("Pedido não encontrado.");

        if (order.Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("Não é possível editar itens de um pedido confirmado.");

        if (order.Status == OrderStatus.Canceled)
            throw new InvalidOperationException("Pedido cancelado não pode ser alterado.");

        var item = await _itemRepository.GetByIdAsync(itemId)
            ?? throw new KeyNotFoundException("Item não encontrado.");

        if (item.OrderId != orderId)
            throw new InvalidOperationException("Item não pertence a este pedido.");

        await _itemRepository.DeleteAsync(itemId);

        order.Items.Remove(item);
        order.RecalculateTotal();
        await _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Pedido não encontrado.");

        if (order.Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("Pedido confirmado não pode ser excluído.");

        await _orderRepository.DeleteAsync(id);
    }

    private static OrderDto MapToDto(Order o) => new()
    {
        Id = o.Id,
        CustomerId = o.CustomerId,
        CustomerName = o.Customer?.Name ?? string.Empty,
        Status = o.Status,
        CreatedAt = o.CreatedAt,
        Total = o.Total,
        Items = o.Items.Select(MapItemToDto).ToList()
    };

    private static OrderItemDto MapItemToDto(OrderItem i) => new()
    {
        Id = i.Id,
        OrderId = i.OrderId,
        Description = i.Description,
        Quantity = i.Quantity,
        UnitPrice = i.UnitPrice,
        LineTotal = i.LineTotal
    };
}
