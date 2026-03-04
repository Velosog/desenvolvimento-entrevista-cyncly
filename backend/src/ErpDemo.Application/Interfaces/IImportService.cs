using ErpDemo.Application.DTOs;

namespace ErpDemo.Application.Interfaces;

public interface IImportService
{
    Task<ImportResultDto> ImportCustomersFromCsvAsync(Stream csvStream);
}
