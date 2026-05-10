using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SatoraCaffeRestaurantTracking.Models;
//Tablolar, ilişkiler, trigger’lar, view’lar ve stored procedure sonuçları buradan yönetilir.
//Controller ve Service katmanları veriye bu sınıf üzerinden erişir.
public partial class CafeContext : DbContext
{

    public CafeContext()
    {
    }

    public CafeContext(DbContextOptions<CafeContext> options)
        : base(options)
    {
    }

    // TABLO KARŞILIKLARI
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<DeliveryOrder> DeliveryOrders { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Reservation> Reservations { get; set; }
    public virtual DbSet<RestaurantTable> RestaurantTables { get; set; }
    public virtual DbSet<ServiceType> ServiceTypes { get; set; }
    public virtual DbSet<Staff> Staff { get; set; }
    public virtual DbSet<StaffLog> StaffLogs { get; set; }
    public virtual DbSet<StaffRole> StaffRoles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Abouts> Abouts { get; set; }
    public virtual DbSet<ContactInfos> ContactInfos { get; set; }
    public virtual DbSet<Messages> Messages { get; set; }
    //stored procedure sonuçlarını ham veri olarak alır
    public virtual DbSet<DashboardRawStats> DashboardRawStats { get; set; }
    public virtual DbSet<BestSellerRawStats> BestSellerRawStats { get; set; }

    // --- OWNER PANEL RAPOR MODELLERİ ---
    //
    public virtual DbSet<DashboardStatsModel> DashboardStatsReports { get; set; }
    public virtual DbSet<BestSellerModel> BestSellerReports { get; set; }
    public virtual DbSet<StaffPerformanceModel> StaffPerformanceReports { get; set; }
    public virtual DbSet<StockReportModel> StockReports { get; set; }
    public virtual DbSet<InactiveUserModel> InactiveUserReports { get; set; }
    public virtual DbSet<TransactionModel> TransactionReports { get; set; }
    //VERİ TABANI BAĞLANTISI!!!!!!
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
     /*  => optionsBuilder.UseSqlServer("Server=DESKTOP-9GMADKV;Database=SatoraCaffeRestaurantDB;Trusted_Connection=True;TrustServerCertificate=True;");*/
    //Kolon Ayarları
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FirstOrderDate).HasColumnType("datetime");
            entity.Property(e => e.LastOrderDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.Telephone).HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<DeliveryOrder>(entity =>
        {
            // --- TRIGGER TANIMLAMASI ---
            entity.ToTable("DeliveryOrders", tb => tb.HasTrigger("trg_AutoCloseDelivery"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");

            entity.HasOne(d => d.Order).WithMany(p => p.DeliveryOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryOrders_Orders");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            // ---TRIGGER TANIMLAMASI ---
            entity.ToTable("Orders", tb => tb.HasTrigger("trg_AutoTableStatus"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.CloseDate).HasColumnType("datetime");

            entity.Property(e => e.TableId).HasColumnName("TableID").IsRequired(false);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID").IsRequired(false);

            entity.Property(e => e.ServiceTypeId).HasColumnName("ServiceTypeID");
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Orders_ServiceTypes");

            entity.HasOne(d => d.Staff).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Orders_Staff");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Orders_RestaurantTables");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            // ---TRIGGER TANIMLAMASI---
            entity.ToTable("OrderDetails", tb => tb.HasTrigger("trg_Stock_DecreaseOnAdd"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payments_Orders");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Payments_PaymentMethods");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MethodName).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            //Tablodaki Trigger'lar yüzünden oluşan EF Core kayıt hatasını (OUTPUT sorunu) engellemek için eklendi.
            entity.ToTable("Products", tb => tb.HasTrigger("trg_ProductFix"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ReservationDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.TableId).HasColumnName("TableID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Customers");

            entity.HasOne(d => d.Table).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_RestaurantTables");
        });

        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ServiceTypeId).HasColumnName("ServiceTypeID");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.RestaurantTables)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RestaurantTables_ServiceTypes");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ServiceName).HasMaxLength(50);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.Staff)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staff_StaffRoles");

            entity.HasOne(d => d.User).WithMany(p => p.Staff)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staff_Users");
        });

        modelBuilder.Entity<StaffLog>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Staff).WithMany(p => p.StaffLogs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StaffLogs_Staff");
        });

        modelBuilder.Entity<StaffRole>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleNmae).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_UserRoles");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Abouts>(entity =>
        {
            entity.ToTable("Abouts");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<ContactInfos>(entity =>
        {
            entity.ToTable("ContactInfos");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Telephone).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Messages>(entity =>
        {
            entity.ToTable("Messages");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SendDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Messages)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Messages_Customers");
        });

        // --- RAPOR TANIMLAMALARI (Keyless Entity) ---
        modelBuilder.Entity<DashboardStatsModel>().HasNoKey().ToView(null);
        modelBuilder.Entity<BestSellerModel>().HasNoKey().ToView(null);
        modelBuilder.Entity<StaffPerformanceModel>().HasNoKey().ToView(null);
        modelBuilder.Entity<StockReportModel>().HasNoKey().ToView(null);
        modelBuilder.Entity<InactiveUserModel>().HasNoKey().ToView(null);
        modelBuilder.Entity<TransactionModel>().HasNoKey().ToView("vw_OrderDetailsFull");

        modelBuilder.Entity<DashboardRawStats>().HasNoKey();
        modelBuilder.Entity<BestSellerRawStats>().HasNoKey();

        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}