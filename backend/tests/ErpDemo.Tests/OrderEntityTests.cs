using ErpDemo.Domain.Entities;
using FluentAssertions;

namespace ErpDemo.Tests;

public class OrderEntityTests
{
    [Fact]
    public void RecalculateTotal_ShouldSumAllLineTotals()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Items = new List<OrderItem>
            {
                new() { Quantity = 2, UnitPrice = 100m },
                new() { Quantity = 3, UnitPrice = 50m },
                new() { Quantity = 1, UnitPrice = 200m }
            }
        };

        // Act
        order.RecalculateTotal();

        // Assert
        order.Total.Should().Be(550m);
    }

    [Fact]
    public void OrderItem_LineTotal_ShouldBeQuantityTimesUnitPrice()
    {
        var item = new OrderItem { Quantity = 5, UnitPrice = 30.50m };
        item.LineTotal.Should().Be(152.50m);
    }
}
