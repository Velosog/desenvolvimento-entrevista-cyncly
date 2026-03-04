using ErpDemo.Application.DTOs;
using ErpDemo.Application.Services;
using ErpDemo.Domain.Entities;
using ErpDemo.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ErpDemo.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repoMock;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _repoMock = new Mock<ICustomerRepository>();
        _service = new CustomerService(_repoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCustomer_WhenDocumentIsUnique()
    {
        // Arrange
        _repoMock.Setup(r => r.DocumentExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Customer>())).ReturnsAsync((Customer c) => c);

        var dto = new CreateCustomerDto
        {
            Name = "Test Customer",
            Document = "12345678901",
            Email = "test@test.com",
            Phone = "(11) 99999-0000"
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Customer");
        result.Document.Should().Be("12345678901");
        result.IsActive.Should().BeTrue();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenDocumentExists()
    {
        // Arrange
        _repoMock.Setup(r => r.DocumentExistsAsync("12345678901", null)).ReturnsAsync(true);

        var dto = new CreateCustomerDto
        {
            Name = "Test",
            Document = "12345678901",
            Email = "test@test.com",
            Phone = ""
        };

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*documento*");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenCustomerNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);

        var dto = new UpdateCustomerDto
        {
            Name = "Updated",
            Document = "12345678901",
            Email = "test@test.com",
            Phone = "",
            IsActive = true
        };

        // Act & Assert
        await _service.Invoking(s => s.UpdateAsync(Guid.NewGuid(), dto))
            .Should().ThrowAsync<KeyNotFoundException>();
    }
}
