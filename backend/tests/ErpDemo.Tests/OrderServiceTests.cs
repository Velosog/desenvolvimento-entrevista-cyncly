using ErpDemo.Application.DTOs;
using ErpDemo.Application.Services;
using ErpDemo.Domain.Entities;
using ErpDemo.Domain.Enums;
using ErpDemo.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ErpDemo.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IOrderItemRepository> _itemRepoMock;
    private readonly Mock<ICustomerRepository> _customerRepoMock;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _itemRepoMock = new Mock<IOrderItemRepository>();
        _customerRepoMock = new Mock<ICustomerRepository>();
        _service = new OrderService(_orderRepoMock.Object, _itemRepoMock.Object, _customerRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCalculateTotal()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

        var dto = new CreateOrderDto
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<CreateOrderItemDto>
            {
                new() { Description = "Item 1", Quantity = 2, UnitPrice = 100.00m },
                new() { Description = "Item 2", Quantity = 1, UnitPrice = 50.00m }
            }
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Total.Should().Be(250.00m);
        result.Items.Should().HaveCount(2);
        result.Status.Should().Be(OrderStatus.Draft);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenCanceledOrderIsModified()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = OrderStatus.Canceled,
            Customer = new Customer { Name = "Test" },
            Items = new List<OrderItem>()
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        // Act & Assert
        await _service.Invoking(s => s.UpdateStatusAsync(order.Id, new UpdateOrderStatusDto { Status = OrderStatus.Confirmed }))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cancelado*");
    }

    [Fact]
    public async Task AddItemAsync_ShouldThrow_WhenOrderIsConfirmed()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = OrderStatus.Confirmed,
            Customer = new Customer { Name = "Test" },
            Items = new List<OrderItem>()
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var itemDto = new CreateOrderItemDto
        {
            Description = "New Item",
            Quantity = 1,
            UnitPrice = 100.00m
        };

        // Act & Assert
        await _service.Invoking(s => s.AddItemAsync(order.Id, itemDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*confirmado*");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCustomerNotFound()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var dto = new CreateOrderDto
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<CreateOrderItemDto>
            {
                new() { Description = "Item", Quantity = 1, UnitPrice = 10m }
            }
        };

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(dto))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateStatusAsync_ConfirmedToDraft_ShouldThrow()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = OrderStatus.Confirmed,
            Customer = new Customer { Name = "Test" },
            Items = new List<OrderItem>()
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        // Act & Assert
        await _service.Invoking(s => s.UpdateStatusAsync(order.Id, new UpdateOrderStatusDto { Status = OrderStatus.Draft }))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*rascunho*");
    }
}
