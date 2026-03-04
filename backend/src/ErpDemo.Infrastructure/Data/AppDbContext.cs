using ErpDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpDemo.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Entity).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(20).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Before).HasColumnType("nvarchar(max)");
            entity.Property(e => e.After).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => new { e.Entity, e.EntityId });
            entity.HasIndex(e => e.Timestamp);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Document).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.Document).IsUnique();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Ignore(e => e.LineTotal);
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var customer1Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        var customer2Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
        var customer3Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = customer1Id, Name = "Empresa Alpha Ltda", Document = "12345678000190", Email = "contato@alpha.com", Phone = "(11) 99999-0001", CreatedAt = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc), IsActive = true },
            new Customer { Id = customer2Id, Name = "João Silva", Document = "12345678901", Email = "joao@email.com", Phone = "(21) 98888-0002", CreatedAt = new DateTime(2024, 2, 20, 14, 30, 0, DateTimeKind.Utc), IsActive = true },
            new Customer { Id = customer3Id, Name = "Maria Santos ME", Document = "98765432000111", Email = "maria@santos.com", Phone = "(31) 97777-0003", CreatedAt = new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc), IsActive = false }
        );

        var order1Id = Guid.Parse("d4e5f6a7-b8c9-0123-def0-234567890123");
        var order2Id = Guid.Parse("e5f6a7b8-c9d0-1234-ef01-345678901234");

        modelBuilder.Entity<Order>().HasData(
            new Order { Id = order1Id, CustomerId = customer1Id, Status = Domain.Enums.OrderStatus.Draft, CreatedAt = new DateTime(2024, 6, 1, 10, 0, 0, DateTimeKind.Utc), Total = 1750.00m },
            new Order { Id = order2Id, CustomerId = customer2Id, Status = Domain.Enums.OrderStatus.Confirmed, CreatedAt = new DateTime(2024, 6, 15, 14, 0, 0, DateTimeKind.Utc), Total = 450.00m }
        );

        modelBuilder.Entity<OrderItem>().HasData(
            new { Id = Guid.Parse("f6a7b8c9-d0e1-2345-f012-456789012345"), OrderId = order1Id, Description = "Licença Software ERP - Anual", Quantity = 1, UnitPrice = 1500.00m },
            new { Id = Guid.Parse("a7b8c9d0-e1f2-3456-0123-567890123456"), OrderId = order1Id, Description = "Suporte Técnico - Mensal", Quantity = 1, UnitPrice = 250.00m },
            new { Id = Guid.Parse("b8c9d0e1-f2a3-4567-1234-678901234567"), OrderId = order2Id, Description = "Consultoria de Implantação", Quantity = 3, UnitPrice = 150.00m }
        );
    }
}
