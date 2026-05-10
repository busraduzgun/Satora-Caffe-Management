using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatoraCaffeRestaurantTracking.Migrations
{
    /// <inheritdoc />
    public partial class ManualFix : Migration
    {
        /// <inheritdoc />

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- 1. ORDERS TABLOSU DÜZELTMELERİ ---
            // CloseDate ekle (Eğer yoksa)
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'CloseDate' AND Object_ID = Object_ID(N'Orders'))
                BEGIN
                    ALTER TABLE Orders ADD CloseDate datetime NULL
                END
            ");

            // Nullable yap (Hata vermez, tekrar çalışabilir)
            migrationBuilder.Sql("ALTER TABLE Orders ALTER COLUMN TableID int NULL");
            migrationBuilder.Sql("ALTER TABLE Orders ALTER COLUMN CustomerID int NULL");

            // --- 2. PAYMENTS TABLOSU DÜZELTMELERİ (AKILLI KONTROL) ---
            migrationBuilder.Sql(@"
                -- Kontrol: Hedeflenen 'PaymentDate' sütunu ZATEN VAR MI?
                IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'PaymentDate' AND Object_ID = Object_ID(N'Payments'))
                BEGIN
                    -- Zaten varsa hiçbir şey yapma. Her şey yolunda.
                    PRINT 'PaymentDate zaten mevcut.'
                END
                ELSE
                BEGIN
                    -- PaymentDate yok. Peki eski 'Date' sütunu var mı?
                    IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'Date' AND Object_ID = Object_ID(N'Payments'))
                    BEGIN
                        -- Eski isim var, yenisi yok -> İSMİNİ DEĞİŞTİR.
                        EXEC sp_rename 'Payments.Date', 'PaymentDate', 'COLUMN';
                    END
                    ELSE
                    BEGIN
                        -- Ne eskisi var ne yenisi -> SIFIRDAN OLUŞTUR.
                        ALTER TABLE Payments ADD PaymentDate datetime NOT NULL DEFAULT GETDATE()
                    END
                END
            ");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abouts");

            migrationBuilder.DropTable(
                name: "ContactInfos");

            migrationBuilder.DropTable(
                name: "DeliveryOrders");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "StaffLogs");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "RestaurantTables");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "StaffRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
