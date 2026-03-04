using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Relatórios")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Retorna relatório resumo com total de pedidos, receita, pedidos por status e top clientes
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(SummaryReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SummaryReportDto>> GetSummary()
    {
        var report = await _reportService.GetSummaryAsync();
        return Ok(report);
    }
}
