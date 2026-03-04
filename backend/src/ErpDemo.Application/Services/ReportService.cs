using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using ErpDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpDemo.Application.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SummaryReportDto> GetSummaryAsync()
    {
        var totalOrders = await _context.Orders.CountAsync();

        var totalRevenue = await _context.Orders
            .Where(o => o.Status == Domain.Enums.OrderStatus.Confirmed)
            .SumAsync(o => o.Total);

        var ordersByStatus = await _context.Orders
            .GroupBy(o => o.Status)
            .Select(g => new OrdersByStatusDto
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                Total = g.Sum(o => o.Total)
            })
            .ToListAsync();

        var topCustomers = await _context.Orders
            .Include(o => o.Customer)
            .GroupBy(o => new { o.CustomerId, o.Customer.Name })
            .Select(g => new TopCustomerDto
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.Name,
                OrderCount = g.Count(),
                TotalSpent = g.Sum(o => o.Total)
            })
            .OrderByDescending(c => c.TotalSpent)
            .Take(10)
            .ToListAsync();

        return new SummaryReportDto
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            OrdersByStatus = ordersByStatus,
            TopCustomers = topCustomers
        };
    }
}
