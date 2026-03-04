namespace ErpDemo.Application.DTOs;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? Before { get; set; }
    public string? After { get; set; }
    public DateTime Timestamp { get; set; }
}
