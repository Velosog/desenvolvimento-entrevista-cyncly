using System.Text.Json;
using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using ErpDemo.Domain.Entities;
using ErpDemo.Domain.Interfaces;

namespace ErpDemo.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _repository;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public AuditService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(string entity, string entityId, string action, string userId, object? before, object? after)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            Entity = entity,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            Before = before != null ? JsonSerializer.Serialize(before, JsonOptions) : null,
            After = after != null ? JsonSerializer.Serialize(after, JsonOptions) : null,
            Timestamp = DateTime.UtcNow
        };

        await _repository.AddAsync(log);
    }

    public async Task<PagedResult<AuditLogDto>> GetPagedAsync(
        string? entity, string? entityId, string? action, string? userId,
        DateTime? fromDate, DateTime? toDate, int page, int pageSize)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            entity, entityId, action, userId, fromDate, toDate, page, pageSize);

        return new PagedResult<AuditLogDto>
        {
            Items = items.Select(a => new AuditLogDto
            {
                Id = a.Id,
                Entity = a.Entity,
                EntityId = a.EntityId,
                Action = a.Action,
                UserId = a.UserId,
                Before = a.Before,
                After = a.After,
                Timestamp = a.Timestamp
            }),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
