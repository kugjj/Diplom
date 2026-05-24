using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WarehouseApp.Controllers;

namespace WarehouseApp.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<DeficitDto> DeficitDtos { get; set; }

    public virtual DbSet<DocStatus> DocStatuses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<InboundInvoice> InboundInvoices { get; set; }

    public virtual DbSet<InventoryAct> InventoryActs { get; set; }

    public virtual DbSet<InventoryDetail> InventoryDetails { get; set; }

    public virtual DbSet<OutboundInvoice> OutboundInvoices { get; set; }

    public virtual DbSet<PriceHistory> PriceHistories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReturnsToSupplier> ReturnsToSuppliers { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }
    public virtual DbSet<WarehouseStatsDto> WarehouseStats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 
        => optionsBuilder.UseSqlServer("Server=ECHIPS-TRAVEL\\MSSQLSERVER01;Database=WarehouseBD;Trusted_Connection=True;TrustServerCertificate=True");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B8E91B13B");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<DocStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__DocStatu__C8EE204358929334");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1BF4E91F4");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Employees)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__Employees__Wareh__398D8EEE");
        });

        modelBuilder.Entity<InboundInvoice>(entity =>
        {
            entity.HasKey(e => e.InboundId).HasName("PK__InboundI__B4DB7A95E81EAD67");

            entity.ToTable(tb => tb.HasTrigger("trg_StockPlus"));

            entity.Property(e => e.InboundId).HasColumnName("InboundID");
            entity.Property(e => e.DateReceived)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Employee).WithMany(p => p.InboundInvoices)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__InboundIn__Emplo__4AB81AF0");

            entity.HasOne(d => d.Product).WithMany(p => p.InboundInvoices)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__InboundIn__Produ__49C3F6B7");

            entity.HasOne(d => d.Supplier).WithMany(p => p.InboundInvoices)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__InboundIn__Suppl__48CFD27E");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InboundInvoices)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__InboundIn__Wareh__4BAC3F29");
        });

        modelBuilder.Entity<InventoryAct>(entity =>
        {
            entity.HasKey(e => e.ActId).HasName("PK__Inventor__AF1624FCD19F82B8");

            entity.Property(e => e.ActId).HasColumnName("ActID");
            entity.Property(e => e.CheckDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Employee).WithMany(p => p.InventoryActs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Inventory__Emplo__619B8048");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryActs)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__Inventory__Wareh__60A75C0F");
        });

        modelBuilder.Entity<InventoryDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__Inventor__135C314D35DB82F6");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.ActId).HasColumnName("ActID");
            entity.Property(e => e.DiffQty).HasComputedColumnSql("([FactQty]-[SystemQty])", false);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Act).WithMany(p => p.InventoryDetails)
                .HasForeignKey(d => d.ActId)
                .HasConstraintName("FK__Inventory__ActID__656C112C");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Inventory__Produ__66603565");
        });

        modelBuilder.Entity<OutboundInvoice>(entity =>
        {
            entity.HasKey(e => e.OutboundId).HasName("PK__Outbound__3518456153EB284C");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_CheckStockBeforeOutbound");
                    tb.HasTrigger("trg_StockMinus");
                });

            entity.Property(e => e.OutboundId).HasColumnName("OutboundID");
            entity.Property(e => e.DateShipped)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SellingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Employee).WithMany(p => p.OutboundInvoices)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__OutboundI__Emplo__5070F446");

            entity.HasOne(d => d.Product).WithMany(p => p.OutboundInvoices)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__OutboundI__Produ__4F7CD00D");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.OutboundInvoices)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__OutboundI__Wareh__5165187F");
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__PriceHis__4D7B4ADDDD8B1C26");

            entity.ToTable("PriceHistory");

            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NewPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OldPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.PriceHistories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__PriceHist__Produ__5535A963");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED70E0FC42");

            entity.ToTable(tb => tb.HasTrigger("trg_NotifyLowStock"));

            entity.HasIndex(e => e.Sku, "UQ__Products__CA1ECF0D29D02198").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CurrentStock).HasDefaultValue(0);
            entity.Property(e => e.Price)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__4316F928");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK__Products__UnitID__440B1D61");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F04A295AAE5");

            entity.Property(e => e.ReservationId).HasColumnName("ReservationID");
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Product).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Reservati__Produ__5CD6CB2B");
        });

        modelBuilder.Entity<ReturnsToSupplier>(entity =>
        {
            entity.HasKey(e => e.ReturnId).HasName("PK__ReturnsT__F445E98842BA04A6");

            entity.ToTable("ReturnsToSupplier");

            entity.Property(e => e.ReturnId).HasColumnName("ReturnID");
            entity.Property(e => e.InboundId).HasColumnName("InboundID");
            entity.Property(e => e.ReturnDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReturnReason).HasMaxLength(255);

            entity.HasOne(d => d.Inbound).WithMany(p => p.ReturnsToSuppliers)
                .HasForeignKey(d => d.InboundId)
                .HasConstraintName("FK__ReturnsTo__Inbou__693CA210");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE66694C1EBF756");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("INN");
            entity.Property(e => e.SupplierName).HasMaxLength(200);
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__SystemLo__5E5499A88B3A45EB");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.ActionTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

            entity.HasOne(d => d.Employee).WithMany(p => p.SystemLogs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__SystemLog__Emplo__59063A47");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Units__44F5EC95D8F3A9DE");

            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.UnitName).HasMaxLength(20);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__2608AFD995E595BB");

            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.WarehouseName).HasMaxLength(100);
        });
        modelBuilder.Entity<DeficitDto>().HasNoKey().ToView(null);

        OnModelCreatingPartial(modelBuilder);


        modelBuilder.Entity<WarehouseStatsDto>().HasNoKey().ToView(null);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
