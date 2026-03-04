using ErpDemo.Domain.Entities;

namespace ErpDemo.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetPagedAsync(
        string? entity, string? entityId, string? action, string? userId,
        DateTime? fromDate, DateTime? toDate, int page, int pageSize);
}
