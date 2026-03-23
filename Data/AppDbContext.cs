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

            // MySQL ve SQL Server gibi ilişkisel DB'ler için hassasiyet ayarları
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.IsActive).HasDefaultValue(true);
        });

        // ── Order Konfigürasyonu ──
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            // Ondalıklı tutarların para birimi (18 tam, 2 kuruş) olarak saklanması için
            entity.Property(o => o.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");

            // Foreign Key ilişkisi: Order → Product
            // Restrict: Ürün silinirse siparişler silinmemeli
            entity.HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Not: Seed Data'lar yeni versiyonda (MySQL geçişi sonrası) kaldırılmıştır.
    }
}
