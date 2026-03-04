using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace ErpDemo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Document = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Name", "Document", "Email", "Phone", "CreatedAt", "IsActive" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "Empresa Alpha Ltda", "12345678000190", "contato@alpha.com", "(11) 99999-0001", new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc), true },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), "João Silva", "12345678901", "joao@email.com", "(21) 98888-0002", new DateTime(2024, 2, 20, 14, 30, 0, DateTimeKind.Utc), true },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), "Maria Santos ME", "98765432000111", "maria@santos.com", "(31) 97777-0003", new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc), false }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerId", "Status", "CreatedAt", "Total" },
                values: new object[,]
                {
                    { new Guid("d4e5f6a7-b8c9-0123-def0-234567890123"), new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "Draft", new DateTime(2024, 6, 1, 10, 0, 0, DateTimeKind.Utc), 1750.00m },
                    { new Guid("e5f6a7b8-c9d0-1234-ef01-345678901234"), new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), "Confirmed", new DateTime(2024, 6, 15, 14, 0, 0, DateTimeKind.Utc), 450.00m }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "Description", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("f6a7b8c9-d0e1-2345-f012-456789012345"), new Guid("d4e5f6a7-b8c9-0123-def0-234567890123"), "Licença Software ERP - Anual", 1, 1500.00m },
                    { new Guid("a7b8c9d0-e1f2-3456-0123-567890123456"), new Guid("d4e5f6a7-b8c9-0123-def0-234567890123"), "Suporte Técnico - Mensal", 1, 250.00m },
                    { new Guid("b8c9d0e1-f2a3-4567-1234-678901234567"), new Guid("e5f6a7b8-c9d0-1234-ef01-345678901234"), "Consultoria de Implantação", 3, 150.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Document",
                table: "Customers",
                column: "Document",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OrderItems");
            migrationBuilder.DropTable(name: "Orders");
            migrationBuilder.DropTable(name: "Customers");
        }
    }
}
