using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Enums;

namespace OrderManagementApi.Services.Interfaces;

/// <summary>
/// Sipariş işlemleri için sözleşme (interface).
/// Stok yönetimi dahil tüm sipariş iş mantığı bu interface'i implement eden sınıfta yer alır.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Tüm siparişleri ürün bilgisiyle birlikte döndürür.
    /// </summary>
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

    /// <summary>
    /// ID ile tek sipariş getirir. Bulunamazsa null döner.
    /// </summary>
    Task<OrderDto?> GetOrderByIdAsync(int id);

    /// <summary>
    /// Yeni sipariş oluşturur. Stok kontrolü yapar, stoktan düşer.
    /// Stok yetersizse veya ürün bulunamazsa exception fırlatır.
    /// </summary>
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);

    /// <summary>
    /// Sipariş durumunu günceller. Bulunamazsa null döner.
    /// </summary>
    Task<OrderDto?> UpdateOrderStatusAsync(int id, OrderStatus newStatus);

    /// <summary>
    /// Siparişi iptal eder ve stoğu geri yükler. Bulunamazsa false döner.
    /// Zaten iptal edilmiş veya teslim edilmiş siparişlerde exception fırlatır.
    /// </summary>
    Task<bool> CancelOrderAsync(int id);
}
