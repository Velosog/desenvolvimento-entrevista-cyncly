using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Tags("Importação")]
public class ImportsController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportsController(IImportService importService)
    {
        _importService = importService;
    }

    /// <summary>
    /// Importa clientes a partir de arquivo CSV
    /// </summary>
    /// <remarks>
    /// O CSV deve ter as colunas: Name, Document, Email, Phone (opcional).
    /// Separadores aceitos: vírgula (,) ou ponto-e-vírgula (;).
    /// Retorna relatório com inseridos, duplicados e inválidos.
    /// </remarks>
    [HttpPost("customers")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportResultDto>> ImportCustomers(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ProblemDetails
            {
                Title = "Arquivo inválido",
                Detail = "Envie um arquivo CSV válido.",
                Status = StatusCodes.Status400BadRequest
            });

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails
            {
                Title = "Formato inválido",
                Detail = "Apenas arquivos .csv são aceitos.",
                Status = StatusCodes.Status400BadRequest
            });

        using var stream = file.OpenReadStream();
        var result = await _importService.ImportCustomersFromCsvAsync(stream);
        return Ok(result);
    }
}
