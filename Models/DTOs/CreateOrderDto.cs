namespace OrderManagementApi.Models.DTOs;

/// <summary>
/// Müşterinin sipariş oluştururken gönderdiği veri paketi.
/// (Validasyon işlemleri FluentValidation ile ayrı bir sınıfta yapılır).
/// </summary>
public class CreateOrderDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? CustomerNote { get; set; }
}
