using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using ErpDemo.Domain.Entities;
using ErpDemo.Domain.Interfaces;

namespace ErpDemo.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<PagedResult<CustomerDto>> GetPagedAsync(
        string? name, string? document, bool? isActive, int page, int pageSize)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(name, document, isActive, page, pageSize);
        return new PagedResult<CustomerDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        if (await _repository.DocumentExistsAsync(dto.Document))
            throw new InvalidOperationException("Já existe um cliente com este documento.");

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Document = dto.Document,
            Email = dto.Email,
            Phone = dto.Phone,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _repository.AddAsync(customer);
        return MapToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto dto)
    {
        var customer = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Cliente não encontrado.");

        if (await _repository.DocumentExistsAsync(dto.Document, id))
            throw new InvalidOperationException("Já existe outro cliente com este documento.");

        customer.Name = dto.Name;
        customer.Document = dto.Document;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.IsActive = dto.IsActive;

        await _repository.UpdateAsync(customer);
        return MapToDto(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException("Cliente não encontrado.");

        await _repository.DeleteAsync(id);
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Document = c.Document,
        Email = c.Email,
        Phone = c.Phone,
        CreatedAt = c.CreatedAt,
        IsActive = c.IsActive
    };
}
