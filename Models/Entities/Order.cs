using OrderManagementApi.Models.Enums;

namespace OrderManagementApi.Models.Entities;

/// <summary>
/// Veritabanındaki sipariş tablosunu temsil eden entity sınıfı.
/// UnitPrice sipariş anındaki fiyatı korur, böylece ürün fiyatı değişse bile eski siparişler etkilenmez.
/// </summary>
public class Order
{
    public int Id { get; set; }

    // Foreign Key → Product tablosu
    public int ProductId { get; set; }

    // Navigation property
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Sipariş edilen adet sayısı. 0'dan büyük olmalıdır.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Sipariş anındaki birim fiyat. Ürün fiyatı değişse bile bu değer korunur.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Toplam tutar = Quantity × UnitPrice
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Siparişin mevcut durumu. Varsayılan: Pending
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string? CustomerNote { get; set; }
}
