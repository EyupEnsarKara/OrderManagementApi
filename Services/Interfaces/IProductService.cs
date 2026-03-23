using OrderManagementApi.Models.DTOs;

namespace OrderManagementApi.Services.Interfaces;

/// <summary>
/// Ürün işlemleri için sözleşme (interface).
/// Tüm ürün iş mantığı bu interface'i implement eden sınıfta yer alır.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Tüm aktif ürünleri döndürür (IsActive == true).
    /// </summary>
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();

    /// <summary>
    /// ID ile tek ürün getirir. Bulunamazsa null döner.
    /// </summary>
    Task<ProductDto?> GetProductByIdAsync(int id);

    /// <summary>
    /// Yeni ürün oluşturur. Fiyat ve stok validasyonları uygulanır.
    /// </summary>
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);

    /// <summary>
    /// Mevcut ürünü günceller. Bulunamazsa null döner.
    /// </summary>
    Task<ProductDto?> UpdateProductAsync(int id, CreateProductDto dto);

    /// <summary>
    /// Ürünü soft-delete yapar (IsActive = false). Bulunamazsa false döner.
    /// </summary>
    Task<bool> DeleteProductAsync(int id);
}
