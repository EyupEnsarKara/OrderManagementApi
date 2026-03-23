using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;
using OrderManagementApi.Services.Interfaces;

namespace OrderManagementApi.Services.Implementations;

/// <summary>
/// Ürün iş mantığını barındıran servis sınıfı.
/// Validasyon, CRUD ve soft delete işlemleri burada gerçekleşir.
/// </summary>
public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tüm aktif ürünleri listeler (IsActive == true).
    /// </summary>
    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// ID ile tek ürün getirir. Bulunamazsa null döner.
    /// </summary>
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }

    /// <summary>
    /// Yeni ürün oluşturur. Fiyat <= 0 veya stok < 0 ise hata fırlatır.
    /// </summary>
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // Validasyonlar
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Ürün adı boş olamaz.");

        if (dto.Price <= 0)
            throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");

        if (dto.Stock < 0)
            throw new ArgumentException("Stok negatif olamaz.");

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }

    /// <summary>
    /// Mevcut ürünü günceller. Bulunamazsa null döner.
    /// </summary>
    public async Task<ProductDto?> UpdateProductAsync(int id, CreateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return null;

        // Validasyonlar
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Ürün adı boş olamaz.");

        if (dto.Price <= 0)
            throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");

        if (dto.Stock < 0)
            throw new ArgumentException("Stok negatif olamaz.");

        // Alanları güncelle
        product.Name = dto.Name.Trim();
        product.Description = dto.Description?.Trim();
        product.Price = dto.Price;
        product.Stock = dto.Stock;

        await _context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }

    /// <summary>
    /// Ürünü soft-delete yapar (IsActive = false). Veritabanından silmez.
    /// </summary>
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return false;

        product.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}
