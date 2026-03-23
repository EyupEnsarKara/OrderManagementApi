namespace OrderManagementApi.Models.Enums;

/// <summary>
/// Siparişin yaşam döngüsündeki durumlarını temsil eder.
/// </summary>
public enum OrderStatus
{
    Pending = 0,       // Bekliyor - Sipariş yeni oluşturuldu
    Confirmed = 1,     // Onaylandı - Sipariş onaylandı
    Shipped = 2,       // Kargoya Verildi
    Delivered = 3,     // Teslim Edildi
    Cancelled = 4      // İptal Edildi - Stok iadesi yapılır
}
