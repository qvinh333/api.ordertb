using API.Sale.Models;
using API.Sale.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Sale.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public static string Unaccent(string value) => throw new NotSupportedException("This method is for use in LINQ queries only.");

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CurrencyRateHistory> CurrencyRateHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("unaccent");
        modelBuilder
            .HasDbFunction(typeof(AppDbContext).GetMethod(nameof(Unaccent), new[] { typeof(string) })!)
            .HasName("unaccent");

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderCode).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OrderDate).IsRequired();
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Specification).HasColumnType("text");
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.SellingPrice).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.AmountSellingPrice).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PaymentStatus).IsRequired();
            entity.Property(e => e.YuanPrice).HasPrecision(18, 2);
            entity.Property(e => e.ImportPrice).HasPrecision(18, 2);
            entity.Property(e => e.Supplier).HasMaxLength(255);
            entity.Property(e => e.WarehousePayment).HasPrecision(18, 2);
            entity.Property(e => e.ShippingWeightFee).HasPrecision(18, 2);
            entity.Property(e => e.ShippingPaymentDate);
            entity.Property(e => e.RefundAmount).HasPrecision(18, 2);
            entity.Property(e => e.RefundStatus).HasMaxLength(255);
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Deleted).IsRequired();
            
            entity.HasIndex(e => e.OrderCode).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Specification).HasColumnType("text");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.DefaultSellingPrice).HasPrecision(18, 2);
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Deleted).IsRequired();

            entity.HasIndex(e => new { e.CreatedBy, e.ProductCode });
            entity.HasIndex(e => new { e.CreatedBy, e.Name });
        });

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Deleted).IsRequired();

            entity.HasIndex(e => new { e.CreatedBy, e.CustomerCode });
            entity.HasIndex(e => new { e.CreatedBy, e.FullName });
            entity.HasIndex(e => new { e.CreatedBy, e.Phone });
        });

        // Currency rate history configuration
        modelBuilder.Entity<CurrencyRateHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rate).IsRequired().HasPrecision(18, 4);
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => new { e.CreatedBy, e.CreatedAt });
        });
    }
}
