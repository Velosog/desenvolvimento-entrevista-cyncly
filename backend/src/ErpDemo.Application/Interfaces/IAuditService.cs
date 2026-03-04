using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;

namespace ErpDemo.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string entity, string entityId, string action, string userId, object? before, object? after);
    Task<PagedResult<AuditLogDto>> GetPagedAsync(
        string? entity, string? entityId, string? action, string? userId,
        DateTime? fromDate, DateTime? toDate, int page, int pageSize);
}
