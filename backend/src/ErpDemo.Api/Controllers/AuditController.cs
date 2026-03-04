using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Tags("Auditoria")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Lista registros de auditoria com filtros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetAll(
        [FromQuery] string? entity,
        [FromQuery] string? entityId,
        [FromQuery] string? action,
        [FromQuery] string? userId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _auditService.GetPagedAsync(entity, entityId, action, userId, fromDate, toDate, page, pageSize);
        return Ok(result);
    }
}
