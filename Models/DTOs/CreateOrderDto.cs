using System.ComponentModel.DataAnnotations;

namespace OrderManagementApi.Models.DTOs;

/// <summary>
/// Müşterinin sipariş oluştururken gönderdiği veri paketi.
/// UnitPrice, TotalPrice, OrderDate, Status sunucu tarafında hesaplanır.
/// </summary>
public class CreateOrderDto
{
    [Required(ErrorMessage = "Ürün ID'si zorunludur.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Adet zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmalıdır.")]
    public int Quantity { get; set; }

    public string? CustomerNote { get; set; }
}
