using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Models.Entities;

namespace OrderManagementApi.Data;

/// <summary>
/// EF Core DbContext - C# sınıflarını veritabanı tablolarına bağlayan köprü.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Product Konfigürasyonu ──
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // SQL Server'a geçince aktif edilecek
            // entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            // entity.Property(p => p.IsActive).HasDefaultValue(true);
        });

        // ── Order Konfigürasyonu ──
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            // SQL Server'a geçince aktif edilecek
            // entity.Property(o => o.UnitPrice).HasColumnType("decimal(18,2)");
            // entity.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");

            // Foreign Key ilişkisi: Order → Product
            // Restrict: Ürün silinirse siparişler silinmemeli
            entity.HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Seed Data (Demo Verileri) ──
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Kablosuz Mouse",
                Description = "Ergonomik kablosuz mouse",
                Price = 299.99m,
                Stock = 50,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 2,
                Name = "Mekanik Klavye",
                Description = "RGB aydınlatmalı mekanik klavye",
                Price = 899.50m,
                Stock = 30,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 3,
                Name = "USB-C Hub",
                Description = "7-in-1 USB-C çoklayıcı",
                Price = 449.00m,
                Stock = 100,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 4,
                Name = "Monitor Standı",
                Description = "Ayarlanabilir alüminyum stand",
                Price = 599.90m,
                Stock = 25,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 5,
                Name = "Webcam HD",
                Description = "1080p HD webcam, mikrofon dahil",
                Price = 349.00m,
                Stock = 40,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            }
        );
    }
}
