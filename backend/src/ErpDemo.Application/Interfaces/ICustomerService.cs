using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;

namespace ErpDemo.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<PagedResult<CustomerDto>> GetPagedAsync(string? name, string? document, bool? isActive, int page, int pageSize);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto dto);
    Task DeleteAsync(Guid id);
}
