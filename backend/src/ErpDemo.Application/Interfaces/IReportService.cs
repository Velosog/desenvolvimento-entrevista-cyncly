using ErpDemo.Application.DTOs;

namespace ErpDemo.Application.Interfaces;

public interface IReportService
{
    Task<SummaryReportDto> GetSummaryAsync();
}
